using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDA : CPUInstruction
    {
        public override string Mnemonic => "LDA";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.Immediate, 0xA9 },
            { AddressingMode.ZeroPage, 0xA5 },
            { AddressingMode.ZeroPageX, 0xB5 },
            { AddressingMode.IndirectX, 0xA1 },
            { AddressingMode.IndirectY, 0xB1 },
        };

        public override int ArgumentCount => 1;

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    cpu.RegisterA = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    cpu.RegisterA = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.RegisterA = cpu.Memory.Read(args[0], cpu.RegisterX);
                    break;
                case AddressingMode.IndirectX:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectY:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterA & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterA == 0;
        }
    }
}
