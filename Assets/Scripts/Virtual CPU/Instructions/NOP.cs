using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class NOP : CPUInstruction
    {
        public override string Mnemonic => "NOP";
        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xEA, 0) },
        };


        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            // Do nothing
        }
    }
}
