using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class RTS : CPUInstruction
    {
        public override string Mnemonic => "RTS";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x60, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            ushort returnAddress = (ushort)(cpu.PullStack() + (cpu.PullStack() << 8));  // Stack should contain the return address in little-endian
            cpu.ProgramCounter = returnAddress;
        }
    }
}
