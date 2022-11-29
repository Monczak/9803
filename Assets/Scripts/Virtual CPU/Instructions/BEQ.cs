using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BEQ : CPUInstruction
    {
        public override string Mnemonic => "BEQ";
        
        public override bool IsBranch => true;

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0xF0, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (cpu.ZeroFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}