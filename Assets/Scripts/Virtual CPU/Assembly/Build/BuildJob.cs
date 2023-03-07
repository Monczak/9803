using NineEightOhThree.Managers;
using UnityEngine;
using AssemblerResult = NineEightOhThree.VirtualCPU.Assembly.Assembler.Assembler.AssemblerResult;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildJob
    {
        public string ResourceLocation { get; }
        public AssemblerResult Result { get; private set; }
        
        public bool Failed => Result.Errors.Count > 0;

        public BuildJob(string resourceLocation)
        {
            ResourceLocation = resourceLocation;
            Result = null;
        }

        public OperationResult<BuildError> Build()
        {
            var resource = Resources.Load<TextAsset>(ResourceLocation);
            if (resource is null) return OperationResult<BuildError>.Error(BuildErrors.ResourceNotFound(this));
            
            string code = resource.text;
            Result = AssemblerInterface.Assemble(code);
            if (Result.Errors.Count > 0)
                return OperationResult<BuildError>.Error(BuildErrors.JobFailed(this));
            
            return OperationResult<BuildError>.Success();
        }
    }
}