using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CLC : CPUInstruction
    {
        public override string Mnemonic => "CLC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x18, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.CarryFlag = false;
        }
    }
}
