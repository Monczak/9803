using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TXS : CPUInstruction
    {
        public override string Mnemonic => "TXS";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x9A, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.StackPointer = cpu.RegisterX;
        }
    }
}