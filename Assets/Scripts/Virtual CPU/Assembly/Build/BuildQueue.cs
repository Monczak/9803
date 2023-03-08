using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine.Assertions.Must;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildQueue
    {
        private readonly Queue<BuildJob> jobs;

        public delegate void ErrorHandler(Error error);

        private readonly ErrorHandler buildErrorHandler, assemblerErrorHandler;

        private bool debug;

        public BuildQueue(ErrorHandler buildErrorHandler = null, ErrorHandler assemblerErrorHandler = null,
            bool debug = false)
        {
            jobs = new Queue<BuildJob>();
            this.buildErrorHandler = buildErrorHandler;
            this.assemblerErrorHandler = assemblerErrorHandler;
            this.debug = debug;
        }

        public void Add(BuildJob job)
        {
            jobs.Enqueue(job);
        }

        public void Add(string resourceLocation)
        {
            jobs.Enqueue(new BuildJob(resourceLocation, Error, Log, debug: debug));
        }

        private void Error(Error error)
        {
            
        }
        
        private void Log(string log)
        {
            Logger.Log(log);
        }

        public BuildResult Build()
        {
            BuildResult result = new();
            
            // TODO: Think about parallelizing this if jobs aren't dependent on one another
            // (AssemblerInterface only uses one static Assembler, though!)
            while (jobs.TryDequeue(out BuildJob job))
            { 
                var buildResult = job.Build();
                result.AddLogs(job);
                if (buildResult.Failed)
                {
                    BuildError theError = (BuildError)buildResult.TheError;
                    result.FailedJobs.Add(job);
                    result.BuildErrors.Add(theError);

                    buildErrorHandler?.Invoke(theError);

                    if (theError?.AssemblerErrors is not null)
                    {
                        foreach (AssemblerError error in theError.AssemblerErrors)
                        {
                            assemblerErrorHandler?.Invoke(error);
                        }
                    }
                }
                else
                {
                    var mergeResult = result.TryMerge(job);

                    if (mergeResult.Failed)
                    {
                        result.FailedJobs.Add(job);
                        result.BuildErrors.Add((BuildError)mergeResult.TheError);
                        buildErrorHandler?.Invoke((BuildError)mergeResult.TheError);
                    }
                }
            }

            return result;
        }
    }
}