using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildError : Error
    {
        public BuildJob Job { get; }
        public string Message { get; }
        public List<AssemblerError> AssemblerErrors { get; }

        public BuildError(BuildJob job, string message)
        {
            Job = job;
            Message = message;
        }

        public BuildError(BuildJob job, string message, List<AssemblerError> assemblerErrors) : this(job, message)
        {
            AssemblerErrors = assemblerErrors;
        }

        public override string ToString() => $"{Job.ResourceLocation}: {Message}";
    }
}