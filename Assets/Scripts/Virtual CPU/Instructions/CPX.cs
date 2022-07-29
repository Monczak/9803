using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CPX : CPUInstruction
    {
        public override string Mnemonic => "CPX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xE0, 1) },
            { AddressingMode.ZeroPage, new(0xE4, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte a = cpu.RegisterX;
            byte b = 0;

            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    b = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    b = cpu.Memory.Read(args[0]);
                    break;
            }

            cpu.CarryFlag = a >= b;
            cpu.ZeroFlag = a == b;
            cpu.NegativeFlag = BitUtils.GetBit((byte)(a - b), 7) == 1;
        }
    }
}