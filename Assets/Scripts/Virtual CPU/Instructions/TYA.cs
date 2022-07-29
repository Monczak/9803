using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TYA : CPUInstruction
    {
        public override string Mnemonic => "TYA";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x98, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterA = cpu.RegisterY;

            cpu.NegativeFlag = (cpu.RegisterA & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterA == 0;
        }
    }
}