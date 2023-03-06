using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class RTI : CPUInstruction
    {
        public override string Mnemonic => "RTI";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0x40, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte statusRegister = cpu.PullStack();  // Stack should contain the original status register
            cpu.StatusRegister = statusRegister;
            cpu.InterruptDisableFlag = false;
            
            ushort returnAddress = (ushort)(cpu.PullStack() + (cpu.PullStack() << 8));  // Stack should contain the return address in little-endian
            cpu.ProgramCounter = returnAddress;
        }
    }
}
