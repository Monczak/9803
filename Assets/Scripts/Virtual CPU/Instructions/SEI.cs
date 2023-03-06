using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class SEI : CPUInstruction
    {
        public override string Mnemonic => "SEI";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x78, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.InterruptDisableFlag = true;
        }
    }
}