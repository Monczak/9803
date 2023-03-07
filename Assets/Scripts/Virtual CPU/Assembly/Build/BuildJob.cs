using NineEightOhThree.Managers;
using UnityEngine;
using AssemblerResult = NineEightOhThree.VirtualCPU.Assembly.Assembler.Assembler.AssemblerResult;

namespace NineEightOhThree.VirtualCPU.Assembly.Build
{
    public class BuildJob
    {
        public string ResourceLocation { get; }
        public AssemblerResult Result { get; }
        
        public bool Failed => Result.Errors.Count > 0;

        public BuildJob(string resourceLocation)
        {
            ResourceLocation = resourceLocation;
            Result = null;
        }

        public void Build()
        {
            string code = Resources.Load<TextAsset>(ResourceLocation).text;
            var assemblerResult = AssemblerInterface.Assemble(code);
        }
    }
}