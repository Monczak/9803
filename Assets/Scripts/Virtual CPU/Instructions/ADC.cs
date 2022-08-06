using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class ADC : CPUInstruction
    {
        public override string Mnemonic => "ADC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0x69, 1) },
            { AddressingMode.ZeroPage, new(0x65, 1) },
            { AddressingMode.ZeroPageX, new(0x75, 1) },
            { AddressingMode.Absolute, new(0x6D, 2) },
            { AddressingMode.AbsoluteX, new(0x7D, 2) },
            { AddressingMode.AbsoluteY, new(0x79, 2) },
            { AddressingMode.IndirectX, new(0x61, 1) },
            { AddressingMode.IndirectY, new(0x71, 1) },
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
                case AddressingMode.Absolute:
                    b = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]));
                    break;
                case AddressingMode.AbsoluteX:
                    b = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterX);
                    break;
                case AddressingMode.AbsoluteY:
                    b = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterY);
                    break;
                case AddressingMode.IndirectX:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectY:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            int result = a + b + (cpu.CarryFlag ? 1 : 0);
            cpu.RegisterA = (byte)result;

            cpu.NegativeFlag = ((byte)result & 0x80) != 0;
            cpu.OverflowFlag = ((a ^ (byte)result) & (b ^ (byte)result) & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
            cpu.CarryFlag = result >= 0x100;
        }
    }
}