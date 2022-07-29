using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STA : CPUInstruction
    {
        public override string Mnemonic => "STA";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.ZeroPage, 0x85 },
            { AddressingMode.ZeroPageX, 0x95 },
            { AddressingMode.IndirectX, 0x81 },
            { AddressingMode.IndirectY, 0x91 },
        };

        public override int ArgumentCount => 1;

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.ZeroPage:
                    cpu.Memory.Write(args[0], cpu.RegisterA);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.Memory.Write(args[0], offset: cpu.RegisterX, cpu.RegisterA);
                    break;
                case AddressingMode.IndirectX:
                    cpu.Memory.Write(cpu.Memory.Read(args[0], cpu.RegisterX), cpu.RegisterA);
                    break;
                case AddressingMode.IndirectY:
                    cpu.Memory.Write(cpu.Memory.Read(args[0]), offset: cpu.RegisterY, cpu.RegisterA);
                    break;
            }
        }
    }
}

