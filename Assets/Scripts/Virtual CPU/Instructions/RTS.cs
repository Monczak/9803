using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class RTS : CPUInstruction
    {
        public override string Mnemonic => "RTS";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Absolute, new(0x60, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte returnAddress = (byte)(cpu.PullStack() + 1);  // Stack should contain address of next instruction - 1
            cpu.ProgramCounter = returnAddress;
        }
    }
}
