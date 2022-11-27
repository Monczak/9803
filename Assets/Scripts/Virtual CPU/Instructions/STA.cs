using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STA : CPUInstruction
    {
        public override string Mnemonic => "STA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0x85, 1) },
            { AddressingMode.ZeroPageX, new(0x95, 1) },
            { AddressingMode.Absolute, new(0x8D, 2) },
            { AddressingMode.AbsoluteX, new(0x9D, 2) },
            { AddressingMode.AbsoluteY, new(0x99, 2) },
            { AddressingMode.IndexedIndirect, new(0x81, 1) },
            { AddressingMode.IndirectIndexed, new(0x91, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.ZeroPage:
                    cpu.Memory.Write(args[0], cpu.RegisterA);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.Memory.Write(args[0], offset: cpu.RegisterX, cpu.RegisterA);
                    break;
                case AddressingMode.Absolute:
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), cpu.RegisterA);
                    break;
                case AddressingMode.AbsoluteX:
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), offset: cpu.RegisterX, cpu.RegisterA);
                    break;
                case AddressingMode.AbsoluteY:
                    cpu.Memory.Write(BitUtils.FromLittleEndian(args[0], args[1]), offset: cpu.RegisterY, cpu.RegisterA);
                    break;
                case AddressingMode.IndexedIndirect:
                    cpu.Memory.Write(cpu.Memory.Read(args[0], cpu.RegisterX), cpu.RegisterA);
                    break;
                case AddressingMode.IndirectIndexed:
                    cpu.Memory.Write(cpu.Memory.Read(args[0]), offset: cpu.RegisterY, cpu.RegisterA);
                    break;
            }
        }
    }
}

