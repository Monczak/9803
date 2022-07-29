using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class PHP : CPUInstruction
    {
        public override string Mnemonic => "PLA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x08, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.PushStack(cpu.StatusRegister);
        }
    }
}