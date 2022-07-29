using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class JSR : CPUInstruction
    {
        public override string Mnemonic => "JSR";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Absolute, new(0x20, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.PushStack((byte)(cpu.ProgramCounter + 1));  // Address of next instruction - 1

            byte address = args[0];
            if (addressingMode == AddressingMode.Indirect)
                address = cpu.Memory.Read(address);
            cpu.ProgramCounter = address;
        }
    }
}
