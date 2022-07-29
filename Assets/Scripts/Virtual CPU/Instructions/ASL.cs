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
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.CarryFlag = BitUtils.GetBit(cpu.RegisterA, 7) != 0;

            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                    cpu.RegisterA <<= 1;
                    break;
                case AddressingMode.ZeroPage:
                    cpu.Memory.Write(args[0], (byte)(cpu.Memory.Read(args[0]) << 1));
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.Memory.Write(args[0], (byte)(cpu.Memory.Read(args[0], cpu.RegisterX) << 1));
                    break;
            }
        }
    }
}