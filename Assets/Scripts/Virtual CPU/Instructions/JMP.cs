using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class JMP : CPUInstruction
    {
        public override string Mnemonic => "JMP";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0x4C, 1) },
            { AddressingMode.Indirect, new(0x6C, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte address = args[0];
            if (addressingMode == AddressingMode.Indirect)
                address = cpu.Memory.Read(address);
            cpu.ProgramCounter = address;
        }
    }
}
