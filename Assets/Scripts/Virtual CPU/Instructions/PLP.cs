using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class PLP : CPUInstruction
    {
        public override string Mnemonic => "PLP";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x28, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.StatusRegister = cpu.PullStack();
        }
    }
}