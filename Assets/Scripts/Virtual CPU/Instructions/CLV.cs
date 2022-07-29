using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CLV : CPUInstruction
    {
        public override string Mnemonic => "CLV";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xB8, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.OverflowFlag = false;
        }
    }
}
