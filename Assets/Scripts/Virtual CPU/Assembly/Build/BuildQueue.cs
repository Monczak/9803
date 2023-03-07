using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildQueue
    {
        private readonly Queue<BuildJob> jobs;

        public delegate void ErrorHandler(BuildError error);

        private ErrorHandler errorHandler;

        public BuildQueue()
        {
            jobs = new Queue<BuildJob>();
        }

        public void Add(BuildJob job)
        {
            jobs.Enqueue(job);
        }

        public void Add(string resourceLocation)
        {
            jobs.Enqueue(new BuildJob(resourceLocation));
        }

        public BuildResult Build()
        {
            BuildResult result = new();
            
            // TODO: Think about parallelizing this if jobs aren't dependent on one another
            // (AssemblerInterface only uses one static Assembler, though!)
            while (jobs.TryDequeue(out BuildJob job))
            { 
                var buildResult = job.Build();
                if (buildResult.Failed)
                {
                    errorHandler((BuildError)buildResult.TheError);
                }
                else
                {
                    var mergeResult = result.TryMerge(job);

                    if (mergeResult.Failed)
                        errorHandler((BuildError)mergeResult.TheError);
                }
            }

            return result;
        }
    }
}