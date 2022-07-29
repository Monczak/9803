using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class AND : CPUInstruction
    {
        public override string Mnemonic => "AND";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0x29, 1) },
            { AddressingMode.ZeroPage, new(0x25, 1) },
            { AddressingMode.ZeroPageX, new(0x35, 1) },
            { AddressingMode.IndirectX, new(0x21, 1) },
            { AddressingMode.IndirectY, new(0x31, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte a = cpu.RegisterA;
            byte b = 0;

            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    b = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    b = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageX:
                    b = cpu.Memory.Read(args[0], cpu.RegisterX);
                    break;
                case AddressingMode.IndirectX:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0], cpu.RegisterX));
                    break;
                case AddressingMode.IndirectY:
                    b = cpu.Memory.Read(cpu.Memory.Read(args[0]), cpu.RegisterY);
                    break;
            }

            byte result = (byte)(a & b);
            cpu.RegisterA = result;

            cpu.NegativeFlag = (result & 0x80) != 0;
            cpu.ZeroFlag = result == 0;
        }
    }
}