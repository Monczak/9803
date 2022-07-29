using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STX : CPUInstruction
    {
        public override string Mnemonic => "STX";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.ZeroPage, 0x86 },
            { AddressingMode.ZeroPageY, 0x96 },
        };

        public override int ArgumentCount => 1;

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
            }
        }
    }
}

