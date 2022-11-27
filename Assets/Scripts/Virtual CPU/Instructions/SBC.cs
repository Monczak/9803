using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class SBC : CPUInstruction
    {
        public override string Mnemonic => "SBC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xE9, 1) },
            { AddressingMode.ZeroPage, new(0xE5, 1) },
            { AddressingMode.ZeroPageX, new(0xF5, 1) },
            { AddressingMode.Absolute, new(0xED, 2) },
            { AddressingMode.AbsoluteX, new(0xFD, 2) },
            { AddressingMode.AbsoluteY, new(0xF9, 2) },
            { AddressingMode.IndexedIndirect, new(0xE1, 1) },
            { AddressingMode.IndirectIndexed, new(0xF1, 1) },
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
                case AddressingMode.IndexedIndirect:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectIndexed:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            int result = a - b - (byte)(cpu.CarryFlag ? 0 : 1);
            cpu.RegisterA = (byte)result;

            cpu.NegativeFlag = ((byte)result & 0x80) != 0;
            cpu.OverflowFlag = ((a ^ (byte)result) & ((0xFF - b) ^ (byte)result) & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
            cpu.CarryFlag = result > 0;
        }
    }
}