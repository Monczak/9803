using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class INY : CPUInstruction
    {
        public override string Mnemonic => "INY";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xC8, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterY++;

            cpu.NegativeFlag = (cpu.RegisterY & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterY == 0;
        }
    }
}