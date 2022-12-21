using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class JSR : CPUInstruction
    {
        public override string Mnemonic => "JSR";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Absolute, new(0x20, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.PushStack((byte)(cpu.ProgramCounter & 0xFF));
            cpu.PushStack((byte)(cpu.ProgramCounter >> 8));  // Current program counter (this does NOT implement the bug present in the 6502)

            ushort address = BitUtils.FromLittleEndian(args[0], args[1]);
            if (addressingMode == AddressingMode.Indirect)
                address = (ushort)(cpu.Memory.Read(address) + cpu.Memory.Read((ushort)(address + 1)) << 8);
            cpu.ProgramCounter = address;
        }
    }
}
