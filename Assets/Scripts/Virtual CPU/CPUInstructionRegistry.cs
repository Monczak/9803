﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    internal class CPUInstructionRegistry
    {
        private static Dictionary<byte, (CPUInstruction, AddressingMode, CPUInstructionMetadata)> cpuInstructionsByOpcode;
        private static Dictionary<string, HashSet<(CPUInstruction, AddressingMode, CPUInstructionMetadata)>> cpuInstructionsByMnemonic;

        public static void RegisterInstructions()
        {
            cpuInstructionsByOpcode = new();
            cpuInstructionsByMnemonic = new();
            int instructionCount = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(CPUInstruction)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CPUInstruction))))
            {
                CPUInstruction instruction = (CPUInstruction)Activator.CreateInstance(type);

                if (!cpuInstructionsByMnemonic.ContainsKey(instruction.Mnemonic))
                    cpuInstructionsByMnemonic.Add(instruction.Mnemonic, new());

                foreach (KeyValuePair<AddressingMode, CPUInstructionMetadata> metadata in instruction.Metadata)
                {
                    byte opcode = metadata.Value.Opcode;

                    if (cpuInstructionsByOpcode.ContainsKey(opcode))
                    {
                        Debug.LogError($"Conflicting opcode {opcode:X2} for instruction {instruction.Mnemonic} {metadata.Key} (originally {cpuInstructionsByOpcode[opcode].Item1.Mnemonic} {cpuInstructionsByOpcode[opcode].Item2})");
                        continue;
                    }

                    cpuInstructionsByOpcode.Add(opcode, (instruction, metadata.Key, metadata.Value));
                    cpuInstructionsByMnemonic[instruction.Mnemonic].Add((instruction, metadata.Key, metadata.Value));

                    instructionCount++;
                }
            }

            Debug.Log($"Loaded {instructionCount} instructions");
        }

        public static (CPUInstruction, AddressingMode, CPUInstructionMetadata) GetInstruction(byte opcode)
        {
            if (cpuInstructionsByOpcode.ContainsKey(opcode))
                return cpuInstructionsByOpcode[opcode];
            throw new UnknownOpcodeException(opcode);
        }

        public static HashSet<(CPUInstruction, AddressingMode, CPUInstructionMetadata)> GetInstructions(string mnemonic)
        {
            if (cpuInstructionsByMnemonic.ContainsKey(mnemonic))
                return cpuInstructionsByMnemonic[mnemonic];
            throw new UnknownInstructionException(mnemonic);
        }
    }
}
