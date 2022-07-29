using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDX : CPUInstruction
    {
        public override string Mnemonic => "LDX";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.Immediate, 0xA2 },
            { AddressingMode.ZeroPage, 0xA6 },
            { AddressingMode.ZeroPageY, 0xB6 },
        };

        public override int ArgumentCount => 1;

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    cpu.RegisterX = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    cpu.RegisterX = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageY:
                    cpu.RegisterX = cpu.Memory.Read(args[0], cpu.RegisterY);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterX & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterX == 0;
        }
    }
}
