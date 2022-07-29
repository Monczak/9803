using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TAX : CPUInstruction
    {
        public override string Mnemonic => "TAX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xAA, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterX = cpu.RegisterA;

            cpu.NegativeFlag = (cpu.RegisterX & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterX == 0;
        }
    }
}