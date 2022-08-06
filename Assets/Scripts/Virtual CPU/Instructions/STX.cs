using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STX : CPUInstruction
    {
        public override string Mnemonic => "STX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0x86, 1) },
            { AddressingMode.ZeroPageY, new(0x96, 1) },
            { AddressingMode.Absolute, new(0x8E, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.ZeroPage:
                    cpu.Memory.Write(args[0], cpu.RegisterX);
                    break;
                case AddressingMode.ZeroPageY:
                    cpu.Memory.Write(args[0], offset: cpu.RegisterY, cpu.RegisterX);
                    break;
                case AddressingMode.Absolute:
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterX);
                    break;
            }
        }
    }
}

