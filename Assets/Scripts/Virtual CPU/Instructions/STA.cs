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
            { AddressingMode.IndirectX, new(0x81, 1) },
            { AddressingMode.IndirectY, new(0x91, 1) },
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
                case AddressingMode.IndirectX:
                    cpu.Memory.Write(cpu.Memory.Read(args[0], cpu.RegisterX), cpu.RegisterA);
                    break;
                case AddressingMode.IndirectY:
                    cpu.Memory.Write(cpu.Memory.Read(args[0]), offset: cpu.RegisterY, cpu.RegisterA);
                    break;
            }
        }
    }
}

