using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class STY : CPUInstruction
    {
        public override string Mnemonic => "STY";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0x84, 1) },
            { AddressingMode.ZeroPageX, new(0x94, 1) },
        };

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

