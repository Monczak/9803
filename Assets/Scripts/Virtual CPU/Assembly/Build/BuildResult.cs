using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildResult
    {
        public byte[] Code { get; }
        public bool[] CodeMask { get; }
        public List<string> Logs { get; }
        public List<AssemblerError> Errors { get; }

        private Dictionary<int, BuildJob> codeOrigins;

        public List<BuildJob> FailedJobs { get; }

        public List<BuildError> BuildErrors { get; }
        public bool Failed => BuildErrors.Count > 0;
        
        public BuildResult()
        {
            Code = new byte[65536];
            CodeMask = new bool[65536];

            Logs = new List<string>();
            Errors = new List<AssemblerError>();

            codeOrigins = new Dictionary<int, BuildJob>();
            
            FailedJobs = new List<BuildJob>();
            BuildErrors = new List<BuildError>();
        }

        public OperationResult TryMerge(BuildJob job)
        {
            Logs.AddRange(job.Result.Logs);
            Errors.AddRange(job.Result.Errors);
            
            if (job.Failed)
            {
                FailedJobs.Add(job);
                return OperationResult.Error(Build.BuildErrors.JobFailed(job));
            }
            
            for (int i = 0; i < CodeMask.Length; i++)
            {
                if (CodeMask[i] && job.Result.CodeMask[i])
                    return OperationResult.Error(Build.BuildErrors.OverlappingCode(job, codeOrigins[i], (ushort)i));

                if (job.Result.CodeMask[i])
                {
                    Code[i] = job.Result.Code[i];
                    CodeMask[i] = true;
                    codeOrigins[i] = job;
                }
            }
            
            return OperationResult.Success();
        }
    }
}