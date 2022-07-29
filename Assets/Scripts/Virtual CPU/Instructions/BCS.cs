using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BCS : CPUInstruction
    {
        public override string Mnemonic => "BCS";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0xB0, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (cpu.CarryFlag)
                cpu.ProgramCounter += args[0];
        }
    }
}