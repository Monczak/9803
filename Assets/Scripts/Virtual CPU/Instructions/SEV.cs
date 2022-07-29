using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class SEV : CPUInstruction
    {
        public override string Mnemonic => "SEV";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xD8, 0) },   // CLD in original 6502 ASM, but decimal mode is unimplemented here
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.OverflowFlag = false;
        }
    }
}
