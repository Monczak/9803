using System.Collections.Generic;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class INC : CPUInstruction
    {
        public override string Mnemonic => "INC";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.ZeroPage, new(0xE6, 1) },
            { AddressingMode.ZeroPageX, new(0xF6, 1) },
            { AddressingMode.Absolute, new(0xEE, 2) },
            { AddressingMode.AbsoluteX, new(0xFE, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            ushort address = addressingMode == AddressingMode.Absolute || addressingMode == AddressingMode.AbsoluteX ?
                BitUtils.FromLittleEndian(args[0], args[1]) : args[0];
            if (addressingMode == AddressingMode.ZeroPageX || addressingMode == AddressingMode.AbsoluteX)
                address += cpu.RegisterX;

            byte value = (byte)(cpu.Memory.Read(address) + 1);
            cpu.Memory.Write(address, value);

            cpu.NegativeFlag = (value & 0x80) != 0;
            cpu.ZeroFlag = value == 0;
        }
    }
}