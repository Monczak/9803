using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class PHA : CPUInstruction
    {
        public override string Mnemonic => "PHA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x48, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.PushStack(cpu.RegisterA);
        }
    }
}