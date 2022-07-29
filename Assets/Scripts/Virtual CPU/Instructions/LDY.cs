using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDY : CPUInstruction
    {
        public override string Mnemonic => "LDY";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.Immediate, 0xA0 },
            { AddressingMode.ZeroPage, 0xA4 },
            { AddressingMode.ZeroPageX, 0xB4 },
        };

        public override int ArgumentCount => 1;

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
