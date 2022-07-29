using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class NOP : CPUInstruction
    {
        public override string Mnemonic => "NOP";
        public override Dictionary<AddressingMode, byte> Opcode => new()
        {
            { AddressingMode.Implied, 0xEA },
        };
        public override int ArgumentCount => 0;


        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            // Do nothing
        }
    }
}
