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
            { AddressingMode.IndirectX, new(0xA1, 1) },
            { AddressingMode.IndirectY, new(0xB1, 1) },
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
                case AddressingMode.IndirectX:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectY:
                    cpu.RegisterA = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterA & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterA == 0;
        }
    }
}
