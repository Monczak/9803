using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class PLA : CPUInstruction
    {
        public override string Mnemonic => "PLA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x68, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterA = cpu.PullStack();
        }
    }
}