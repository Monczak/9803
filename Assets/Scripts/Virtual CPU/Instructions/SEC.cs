using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class SEC : CPUInstruction
    {
        public override string Mnemonic => "SEC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x38, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.CarryFlag = true;
        }
    }
}
