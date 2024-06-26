﻿using NineEightOhThree.VirtualCPU.Utilities;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class CPY : CPUInstruction
    {
        public override string Mnemonic => "CPY";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xC0, 1) },
            { AddressingMode.ZeroPage, new(0xC4, 1) },
            { AddressingMode.Absolute, new(0xCC, 2) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            byte a = cpu.RegisterY;
            byte b = 0;

            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    b = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    b = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.Absolute:
                    b = cpu.Memory.Read(BitUtils.FromLittleEndian(args[0], args[1]));
                    break;
            }

            cpu.CarryFlag = a >= b;
            cpu.ZeroFlag = a == b;
            cpu.NegativeFlag = BitUtils.GetBit((byte)(a - b), 7) == 1;
        }
    }
}