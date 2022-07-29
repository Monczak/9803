﻿using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class INX : CPUInstruction
    {
        public override string Mnemonic => "INX";

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Implied, new(0xE8, 0) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            cpu.RegisterX++;

            cpu.NegativeFlag = (cpu.RegisterX & 0x80) != 0;
            cpu.ZeroFlag = cpu.RegisterX == 0;
        }
    }
}