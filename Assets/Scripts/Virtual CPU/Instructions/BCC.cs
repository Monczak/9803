﻿using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BCC : CPUInstruction
    {
        public override string Mnemonic => "BCC";

        public override bool IsBranch => true;

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0x90, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (!cpu.CarryFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}