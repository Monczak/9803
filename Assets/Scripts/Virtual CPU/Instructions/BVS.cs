using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BVS : CPUInstruction
    {
        public override string Mnemonic => "BVS";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0x70, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (cpu.OverflowFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}