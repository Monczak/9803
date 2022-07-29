﻿using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class LDX : CPUInstruction
    {
        public override string Mnemonic => "LDX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Immediate, new(0xA2, 1) },
            { AddressingMode.ZeroPage, new(0xA6, 1) },
            { AddressingMode.ZeroPageY, new(0xB6, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            switch (addressingMode)
            {
                case AddressingMode.Immediate:
                    cpu.RegisterX = args[0];
                    break;
                case AddressingMode.ZeroPage:
                    cpu.RegisterX = cpu.Memory.Read(args[0]);
                    break;
                case AddressingMode.ZeroPageY:
                    cpu.RegisterX = cpu.Memory.Read(args[0], cpu.RegisterY);
                    break;
            }

            cpu.NegativeFlag = (cpu.RegisterX & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterX == 0;
        }
    }
}
