using NineEightOhThree.Managers;
using NineEightOhThree.VirtualCPU.Assembly.Assembler;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildJob
    {
        public string ResourceLocation { get; }
        public AssemblerResult Result { get; private set; }
        
        public bool Failed => Result.Errors.Count > 0;

        private bool debug;

        private readonly ErrorHandler errorHandler;
        private readonly LogHandler logHandler;
        
        public BuildJob(string resourceLocation, ErrorHandler errorHandler = null, LogHandler logHandler = null, bool debug = false)
        {
            ResourceLocation = resourceLocation;
            Result = null;

            this.errorHandler = errorHandler;
            this.logHandler = logHandler;

            this.debug = debug;
        }

        public OperationResult<BuildError> Build()
        {
            var resource = Resources.Load<TextAsset>(ResourceLocation);
            if (resource is null) return OperationResult<BuildError>.Error(BuildErrors.ResourceNotFound(this));
            
            string code = resource.text;
            if (errorHandler is null || logHandler is null)
                Result = AssemblerInterface.Assemble(code, ResourceLocation);
            else
                Result = AssemblerInterface.Assemble(code, errorHandler, logHandler, ResourceLocation);

            if (debug)
                AssemblerInterface.Assembler.LogDebugData();
            
            if (Result.Errors.Count > 0)
                return OperationResult<BuildError>.Error(BuildErrors.AssemblerFailed(this));
            
            return OperationResult<BuildError>.Success();
        }
    }
}