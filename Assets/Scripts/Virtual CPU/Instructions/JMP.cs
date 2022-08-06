using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class JMP : CPUInstruction
    {
        public override string Mnemonic => "JMP";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Absolute, new(0x4C, 2) },
            { AddressingMode.Indirect, new(0x6C, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            ushort address = BitUtils.FromLittleEndian(args[0], args[1]);
            if (addressingMode == AddressingMode.Indirect)
                address = (ushort)(cpu.Memory.Read(address) + cpu.Memory.Read((ushort)(address + 1)) << 8);
            cpu.ProgramCounter = address;
        }
    }
}
