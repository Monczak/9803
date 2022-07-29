using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    internal class CPUInstructionRegistry
    {
        private static Dictionary<byte, (CPUInstruction, AddressingMode)> cpuInstructionsByOpcode;
        private static Dictionary<string, HashSet<(CPUInstruction, AddressingMode)>> cpuInstructionsByMnemonic;

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

                foreach (KeyValuePair<AddressingMode, byte> opcode in instruction.Opcode)
                {
                    cpuInstructionsByOpcode.Add(opcode.Value, (instruction, opcode.Key));
                    cpuInstructionsByMnemonic[instruction.Mnemonic].Add((instruction, opcode.Key));

                    instructionCount++;
                }
            }

            Debug.Log($"Loaded {instructionCount} instructions");
        }

        public static (CPUInstruction, AddressingMode) GetInstruction(byte opcode)
        {
            if (cpuInstructionsByOpcode.ContainsKey(opcode))
                return cpuInstructionsByOpcode[opcode];
            throw new UnknownOpcodeException(opcode);
        }

        public static HashSet<(CPUInstruction, AddressingMode)> GetInstructions(string mnemonic)
        {
            if (cpuInstructionsByMnemonic.ContainsKey(mnemonic))
                return cpuInstructionsByMnemonic[mnemonic];
            throw new UnknownInstructionException(mnemonic);
        }
    }
}
