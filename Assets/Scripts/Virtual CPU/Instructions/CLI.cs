using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CLI : CPUInstruction
    {
        public override string Mnemonic => "CLI";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x58, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.InterruptDisableFlag = false;
        }
    }
}