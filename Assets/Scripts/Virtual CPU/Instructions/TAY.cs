using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TAY : CPUInstruction
    {
        public override string Mnemonic => "TAY";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xA8, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterY = cpu.RegisterA;

            cpu.NegativeFlag = (cpu.RegisterY & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterY == 0;
        }
    }
}