using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class XOR : CPUInstruction
    {
        public override string Mnemonic => "XOR";
        public override List<string> Aliases => new() { "EOR" };

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0x49, 1) },
            { AddressingMode.ZeroPage, new(0x45, 1) },
            { AddressingMode.ZeroPageX, new(0x55, 1) },
            { AddressingMode.Absolute, new(0x4D, 1) },
            { AddressingMode.AbsoluteX, new(0x5D, 1) },
            { AddressingMode.AbsoluteY, new(0x59, 1) },
            { AddressingMode.IndirectX, new(0x41, 1) },
            { AddressingMode.IndirectY, new(0x51, 1) },
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

            byte result = (byte)(a ^ b);
            cpu.RegisterA = result;

            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}