using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LSR : CPUInstruction
    {
        public override string Mnemonic => "LSR";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Accumulator, new(0x4A, 0) },
            { AddressingMode.ZeroPage, new(0x46, 1) },
            { AddressingMode.ZeroPageX, new(0x56, 1) },
            { AddressingMode.Absolute, new(0x4E, 2) },
            { AddressingMode.AbsoluteX, new(0x5E, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte bit0 = (byte)(cpu.RegisterA & 0x1);
            cpu.CarryFlag = bit0 != 0;

            byte result = 0;

            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                    cpu.RegisterA >>= 1;
                    cpu.RegisterA |= (byte)(bit0 << 7);
                    result = cpu.RegisterA;
                    break;
                case AddressingMode.ZeroPage:
                    result = (byte)((cpu.Memory.Read(args[0]) >> 1) | (byte)(bit0 << 7));
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.ZeroPageX:
                    result = (byte)((cpu.Memory.Read(args[0], cpu.RegisterX) >> 1) | (byte)(bit0 << 7));
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.Absolute:
                    result = (byte)((cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1])) >> 1) | (byte)(bit0 << 7));
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.AbsoluteX:
                    result = (byte)((cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterX) >> 1) | (byte)(bit0 << 7));
                    cpu.Memory.Write(args[0], result);
                    break;
            }

            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}