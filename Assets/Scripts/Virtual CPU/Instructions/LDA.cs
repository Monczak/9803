using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDA : CPUInstruction
    {
        public override string Mnemonic => "LDA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xA9, 1) },
            { AddressingMode.ZeroPage, new(0xA5, 1) },
            { AddressingMode.ZeroPageX, new(0xB5, 1) },
            { AddressingMode.Absolute, new(0xAD, 2) },
            { AddressingMode.AbsoluteX, new(0xBD, 2) },
            { AddressingMode.AbsoluteY, new(0xB9, 2) },
            { AddressingMode.IndexedIndirect, new(0xA1, 1) },
            { AddressingMode.IndirectIndexed, new(0xB1, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    cpu.RegisterA = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    cpu.RegisterA = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.RegisterA = cpu.Memory.Read(args[0], cpu.RegisterX);
                    break;
                case AddressingMode.Absolute:
                    cpu.RegisterA = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]));
                    break;
                case AddressingMode.AbsoluteX:
                    cpu.RegisterA = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterX);
                    break;
                case AddressingMode.AbsoluteY:
                    cpu.RegisterA = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterY);
                    break;
                case AddressingMode.IndexedIndirect:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectIndexed:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterA & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterA == 0;
        }
    }
}
