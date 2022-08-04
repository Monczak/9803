using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BNE : CPUInstruction
    {
        public override string Mnemonic => "BNE";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0xD0, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (!cpu.ZeroFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}