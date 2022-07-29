using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class JMP : CPUInstruction
    {
        public override string Mnemonic => "JMP";

        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.Absolute, 0x4C },
            { AddressingMode.Indirect, 0x6C },
        };

        public override int ArgumentCount => 1;

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte address = args[0];
            if (addressingMode == AddressingMode.Indirect)
                address = cpu.Memory.Read(address);
            cpu.ProgramCounter = address;
        }
    }
}
