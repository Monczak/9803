using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildResult
    {
        public byte[] Code { get; }
        public bool[] CodeMask { get; }
        public List<string> Logs { get; }
        public List<AssemblerError> AssemblerErrors { get; }

        private readonly Dictionary<int, BuildJob> codeOrigins;

        public List<BuildJob> FailedJobs { get; }

        public List<BuildError> BuildErrors { get; }
        public Dictionary<BuildJob, List<AssemblerError>> JobAssemblerErrors { get; }

        public bool Failed => BuildErrors.Count > 0 || AssemblerErrors.Count > 0;
        
        public BuildResult()
        {
            Code = new byte[65536];
            CodeMask = new bool[65536];

            Logs = new List<string>();
            AssemblerErrors = new List<AssemblerError>();

            codeOrigins = new Dictionary<int, BuildJob>();
            
            FailedJobs = new List<BuildJob>();
            BuildErrors = new List<BuildError>();

            JobAssemblerErrors = new Dictionary<BuildJob, List<AssemblerError>>();
        }

        public void AddLogs(BuildJob job)
        {
            if (job.Result is not null)
            {
                Logs.AddRange(job.Result.Logs);
                AssemblerErrors.AddRange(job.Result.Errors);

                if (!JobAssemblerErrors.ContainsKey(job))
                    JobAssemblerErrors[job] = new List<AssemblerError>();
                JobAssemblerErrors[job].AddRange(job.Result.Errors);
            }
        }

        public OperationResult TryMerge(BuildJob job)
        {
            if (job.Failed)
            {
                FailedJobs.Add(job);
                return OperationResult.Error(Build.BuildErrors.AssemblerFailed(job));
            }
            
            for (int i = 0; i < CodeMask.Length; i++)
            {
                if (CodeMask[i] && job.Result.AssembledCode.Mask[i])
                    return OperationResult.Error(Build.BuildErrors.OverlappingCode(job, codeOrigins[i], (ushort)i));

                if (job.Result.AssembledCode.Mask[i])
                {
                    Code[i] = job.Result.AssembledCode.Code[i];
                    CodeMask[i] = true;
                    codeOrigins[i] = job;
                }
            }
            
            return OperationResult.Success();
        }
    }
}