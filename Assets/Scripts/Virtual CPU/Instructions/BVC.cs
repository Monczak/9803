using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BVC : CPUInstruction
    {
        public override string Mnemonic => "BVC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0x50, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (!cpu.OverflowFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}