using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU
{
    internal class CPUInstructionRegistry
    {
        private static Dictionary<byte, CPUInstruction> cpuInstructions;

        public static void RegisterInstructions()
        {
            cpuInstructions = new();
            int instructionCount = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(CPUInstruction)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CPUInstruction))))
            {
                CPUInstruction instruction = (CPUInstruction)Activator.CreateInstance(type);
                cpuInstructions.Add(instruction.Opcode, instruction);
                instructionCount++;
            }

            Debug.Log($"Loaded {instructionCount} instructions");
        }

        public static CPUInstruction GetInstruction(byte opcode)
        {
            if (cpuInstructions.ContainsKey(opcode))
                return cpuInstructions[opcode];
            throw new UnknownOpcodeException(opcode);
        }
    }
}
