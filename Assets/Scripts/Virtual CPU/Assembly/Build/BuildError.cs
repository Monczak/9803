using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildError : Error
    {
        public BuildJob Job { get; }
        public string Message { get; }
        public AssemblerError AssemblerError { get; }

        public BuildError(BuildJob job, string message)
        {
            Job = job;
            Message = message;
        }

        public BuildError(BuildJob job, string message, AssemblerError assemblerError) : this(job, message)
        {
            AssemblerError = assemblerError;
        }
    }
}