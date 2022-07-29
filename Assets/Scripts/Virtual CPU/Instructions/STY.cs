using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STY : CPUInstruction
    {
        public override string Mnemonic => "STY";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.ZeroPage, 0x84 },
            { AddressingMode.ZeroPageX, 0x94 },
        };

        public override int ArgumentCount => 1;

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.ZeroPage:
                    cpu.Memory.Write(args[0], cpu.RegisterY);
                    break;
                case AddressingMode.ZeroPageX:
                    cpu.Memory.Write(args[0], offset: cpu.RegisterX, cpu.RegisterY);
                    break;

            }
        }
    }
}

