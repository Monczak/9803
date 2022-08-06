using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class ASL : CPUInstruction
    {
        public override string Mnemonic => "ASL";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Accumulator, new(0x0A, 0) },
            { AddressingMode.ZeroPage, new(0x06, 1) },
            { AddressingMode.ZeroPageX, new(0x16, 1) },
            { AddressingMode.Absolute, new(0x0E, 2) },
            { AddressingMode.AbsoluteX, new(0x1E, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.CarryFlag = BitUtils.GetBit(cpu.RegisterA, 7) != 0;

            byte result = 0;

            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                    cpu.RegisterA <<= 1;
                    result = cpu.RegisterA;
                    break;
                case AddressingMode.ZeroPage:
                    result = (byte)(cpu.Memory.Read(args[0]) << 1);
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.ZeroPageX:
                    result = (byte)(cpu.Memory.Read(args[0], cpu.RegisterX) << 1);
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.Absolute:
                    result = (byte)(cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1])) << 1);
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), result);
                    break;
                case AddressingMode.AbsoluteX:
                    result = (byte)(cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterX) << 1);
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), result);
                    break;
            }

            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}