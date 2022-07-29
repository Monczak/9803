using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class TSX : CPUInstruction
    {
        public override string Mnemonic => "TSX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xBA, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterX = cpu.StackPointer;
        }
    }
}