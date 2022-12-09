using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using ReflectionAssembly = System.Reflection.Assembly;

namespace NineEightOhThree.VirtualCPU
{
    internal static class CPUInstructionRegistry
    {
        private static Dictionary<byte, (CPUInstruction, AddressingMode, CPUInstructionMetadata)> cpuInstructionsByOpcode;
        private static Dictionary<string, List<(CPUInstruction, AddressingMode, CPUInstructionMetadata)>> cpuInstructionsByMnemonic;

        public static void RegisterInstructions()
        {
            cpuInstructionsByOpcode = new Dictionary<byte, (CPUInstruction, AddressingMode, CPUInstructionMetadata)>();
            cpuInstructionsByMnemonic = new Dictionary<string, List<(CPUInstruction, AddressingMode, CPUInstructionMetadata)>>();
            int instructionCount = 0;
            foreach (Type type in ReflectionAssembly.GetAssembly(typeof(CPUInstruction)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CPUInstruction))))
            {
                CPUInstruction instruction = (CPUInstruction)Activator.CreateInstance(type);

                if (!cpuInstructionsByMnemonic.ContainsKey(instruction.Mnemonic))
                    cpuInstructionsByMnemonic.Add(instruction.Mnemonic, new List<(CPUInstruction, AddressingMode, CPUInstructionMetadata)>());
                foreach (var alias in instruction.Aliases.Where(alias => !cpuInstructionsByMnemonic.ContainsKey(alias)))
                    cpuInstructionsByMnemonic.Add(alias, new List<(CPUInstruction, AddressingMode, CPUInstructionMetadata)>());

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
                    foreach (string alias in instruction.Aliases)
                        cpuInstructionsByMnemonic[alias].Add((instruction, metadata.Key, metadata.Value));

                    instructionCount++;
                }
            }

            Debug.Log($"Loaded {instructionCount} instructions");
        }

        public static bool GetInstruction(byte opcode, out (CPUInstruction instruction, AddressingMode addressingMode, CPUInstructionMetadata metadata)? result)
        {
            if (cpuInstructionsByOpcode.ContainsKey(opcode))
            {
                result = cpuInstructionsByOpcode[opcode];
                return true;
            }

            result = null;
            return false;
        }

        public static bool GetInstructions(string mnemonic, out IEnumerable<(CPUInstruction instruction, AddressingMode addressingMode, CPUInstructionMetadata metadata)> result)
        {
            if (cpuInstructionsByMnemonic.ContainsKey(mnemonic.ToUpper()))
            {
                result = cpuInstructionsByMnemonic[mnemonic.ToUpper()];
                return true;
            }

            result = null;
            return false;
        }
    }
}
