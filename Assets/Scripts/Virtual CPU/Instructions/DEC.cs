using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class DEC : CPUInstruction
    {
        public override string Mnemonic => "DEC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0xC6, 1) },
            { AddressingMode.ZeroPageX, new(0xD6, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte address = args[0];
            if (addressingMode == AddressingMode.ZeroPageX)
                address += cpu.RegisterX;

            byte value = (byte)(cpu.Memory.Read(address) - 1);
            cpu.Memory.Write(address, value);

            cpu.NegativeFlag = (value & 0x80) != 0;
            cpu.ZeroFlag = value == 0;
        }
    }
}