using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CMP : CPUInstruction
    {
        public override string Mnemonic => "CMP";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xC9, 1) },
            { AddressingMode.ZeroPage, new(0xC5, 1) },
            { AddressingMode.ZeroPageX, new(0xD5, 1) },
            { AddressingMode.IndirectX, new(0xC1, 1) },
            { AddressingMode.IndirectY, new(0xD1, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte a = cpu.RegisterA;
            byte b = 0;

            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    b = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    b = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageX:
                    b = cpu.Memory.Read(args[0], cpu.RegisterX);
                    break;
                case AddressingMode.IndirectX:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectY:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            cpu.CarryFlag = a >= b;
            cpu.ZeroFlag = a == b;
            cpu.NegativeFlag = BitUtils.GetBit((byte)(a - b), 7) == 1;
        }
    }
}