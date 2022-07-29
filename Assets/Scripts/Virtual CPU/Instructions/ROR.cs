using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class ROR : CPUInstruction
    {
        public override string Mnemonic => "ROR";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Accumulator, new(0x6A, 0) },
            { AddressingMode.ZeroPage, new(0x66, 1) },
            { AddressingMode.ZeroPageX, new(0x76, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte result = 0;
            byte bit0 = 0, value;

            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                    bit0 = (byte)(cpu.RegisterA & 0x1);
                    cpu.RegisterA >>= 1;
                    cpu.RegisterA |= (byte)((cpu.CarryFlag ? 1 : 0) << 7);
                    result = cpu.RegisterA;
                    break;
                case AddressingMode.ZeroPage:
                    value = cpu.Memory.Read(args[0]);
                    bit0 = (byte)(value & 0x1);
                    result = (byte)(value >> 1);
                    result |= (byte)((cpu.CarryFlag ? 1 : 0) << 7);
                    cpu.Memory.Write(args[0], result);
                    break;
                case AddressingMode.ZeroPageX:
                    value = cpu.Memory.Read(args[0], cpu.RegisterX);
                    bit0 = (byte)(value & 0x1);
                    result = (byte)(value >> 1);
                    result |= (byte)((cpu.CarryFlag ? 1 : 0) << 7);
                    cpu.Memory.Write(args[0], result);
                    break;
            }

            cpu.CarryFlag = bit0 != 0;
            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}