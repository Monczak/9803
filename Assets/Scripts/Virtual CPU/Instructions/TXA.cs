using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TXA : CPUInstruction
    {
        public override string Mnemonic => "TXA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x8A, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterA = cpu.RegisterX;

            cpu.NegativeFlag = (cpu.RegisterA & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterA == 0;
        }
    }
}