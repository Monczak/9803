﻿using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Instructions
{
    public class BMI : CPUInstruction
    {
        public override string Mnemonic => "BMI";
        
        public override bool IsBranch => true;

        public override Dictionary<AddressingMode, CPUInstructionMetadata> Metadata => new()
        {
            { AddressingMode.Relative, new(0x30, 1) },
        };

        public override void Execute(CPU cpu, AddressingMode addressingMode)
        {
            if (cpu.NegativeFlag)
                cpu.ProgramCounter = (ushort)(cpu.ProgramCounter + (sbyte)args[0]);
        }
    }
}