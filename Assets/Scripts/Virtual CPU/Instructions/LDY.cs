using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDY : CPUInstruction
    {
        public override string Mnemonic => "LDY";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xA0, 1) },
            { AddressingMode.ZeroPage, new(0xA4, 1) },
            { AddressingMode.ZeroPageX, new(0xB4, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    cpu.RegisterY = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    cpu.RegisterY = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.RegisterY = cpu.Memory.Read(args[0], cpu.RegisterX);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterY & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterY == 0;
        }
    }
}
