using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class ROL : CPUInstruction
    {
        public override string Mnemonic => "ROL";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Accumulator, new(0x2A, 0) },
            { AddressingMode.ZeroPage, new(0x26, 1) },
            { AddressingMode.ZeroPageX, new(0x36, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte result = 0;
            byte bit7 = 0, value;

            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                    bit7 = (byte)(cpu.RegisterA & 0x80);
                    cpu.RegisterA <<= 1;
                    cpu.RegisterA |= (byte)(cpu.CarryFlag ? 1 : 0);
                    result = cpu.RegisterA;
                    break;
                case AddressingMode.ZeroPage:
                    value = cpu.Memory.Read(args[0]);
                    bit7 = (byte)(value & 0x80);
                    result = (byte)(value << 1);
                    result |= (byte)(cpu.CarryFlag ? 1 : 0);
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.ZeroPageX:
                    value = cpu.Memory.Read(args[0], cpu.RegisterX);
                    bit7 = (byte)(value & 0x80);
                    result = (byte)(value << 1);
                    result |= (byte)(cpu.CarryFlag ? 1 : 0);
                    cpu.Memory.Write(args[0], result);
                    break;
            }

            cpu.CarryFlag = bit7 != 0;
            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}