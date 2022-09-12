using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Assembler
    {
        public static string AddressPrefix { get; set; } = "$";
        public static string ImmediatePrefix { get; set; } = "#$";

        public static List<byte> Assemble(string input)
        {
            List<byte> machineCode = new();

            string[] lines = input.Split("\n");
            int lineIndex = 1;
            foreach (string line in lines)
            {
                Debug.Log(line);

                string[] strings = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Ignore comments and everything after a comment
                int index;
                for (index = 0; index < strings.Length; index++)
                    if (strings[index].StartsWith(";"))
                        break;
                strings = strings[..index];

                // String 0 should either be an instruction or a label definition
                // TODO: Labels

                List<(CPUInstruction, AddressingMode, CPUInstructionMetadata)> instructionFamily;
                try
                {
                    instructionFamily = new(CPUInstructionRegistry.GetInstructions(strings[0].ToUpper()));
                }
                catch (UnknownInstructionException e)
                {
                    throw new SyntaxError($"Invalid instruction {e.Mnemonic}", lineIndex);
                }

                // If the argument count is non-zero, string 1 should contain the necessary arguments for the instruction
                // (or a label in case of a JMP, JSR or branch)
                // Else it shouldn't exist

                // Try figuring out the addressing mode from the argument?

                AddressingMode addressingMode;
                if (strings.Length == 1)
                    addressingMode = AddressingMode.Implied;
                else
                {
                    addressingMode = strings[1] switch
                    {
                        "A" => AddressingMode.Accumulator,
                        var str when instructionFamily.Count(i => i.Item2 == AddressingMode.Relative) > 0 && str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 2 => AddressingMode.Relative,
                        var str when str.StartsWith($"({AddressPrefix}") && ExtractHex(str).Length == 2 && str.ToUpper().EndsWith(",X)") => AddressingMode.IndirectX,
                        var str when str.StartsWith($"({AddressPrefix}") && ExtractHex(str).Length == 2 && str.ToUpper().EndsWith("),Y") => AddressingMode.IndirectY,
                        var str when str.StartsWith($"({AddressPrefix}") && ExtractHex(str).Length == 2 && str.EndsWith(")") => AddressingMode.Indirect,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 4 && str.ToUpper().EndsWith(",X") => AddressingMode.AbsoluteX,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 4 && str.ToUpper().EndsWith(",Y") => AddressingMode.AbsoluteY,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 4 => AddressingMode.Absolute,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 2 && str.ToUpper().EndsWith(",X") => AddressingMode.ZeroPageX,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 2 && str.ToUpper().EndsWith(",Y") => AddressingMode.ZeroPageY,
                        var str when str.StartsWith(AddressPrefix) && ExtractHex(str).Length == 2 => AddressingMode.ZeroPage,
                        var str when str.StartsWith(ImmediatePrefix) && ExtractHex(str).Length <= 2 => AddressingMode.Immediate,
                        _ => throw new SyntaxError("Invalid argument", lineIndex)
                    };
                }

                // Check if the specified argument is valid for the instruction
                byte[] hexValues = new byte[0];
                if (addressingMode != AddressingMode.Implied && addressingMode != AddressingMode.Accumulator)
                {
                    hexValues = SplitHex(ExtractHex(strings[1]));
                }

                var instructions = instructionFamily.FindAll(i => i.Item2 == addressingMode);
                (CPUInstruction, AddressingMode, CPUInstructionMetadata) instruction;
                if (instructions.Count == 0)
                    throw new SyntaxError($"Invalid argument for this instruction", lineIndex);
                else if (instructions.Count != 1)
                    throw new SyntaxError($"Ambiguous argument", lineIndex);    // Should never happen if the instruction set is well-defined, but here it is anyway
                else
                    instruction = instructions[0];

                machineCode.Add(instruction.Item3.Opcode);
                foreach (byte b in hexValues)
                    machineCode.Add(b);


                lineIndex++;
            }

            return machineCode;
        }

        private static string ExtractHex(string str)
        {
            return Regex.Match(str, @"(?i)([0-9a-f]+)")?.Value;
        }

        private static byte[] SplitHex(string hexStr, bool littleEndian = true)
        {
            byte[] bytes = new byte[(hexStr.Length + 1) / 2];
            int strPointer = System.Math.Max(0, hexStr.Length - 2);
            int i = 0;

            while (strPointer >= 0)
            {
                bytes[i++] = byte.Parse(hexStr.Substring(strPointer, 2), System.Globalization.NumberStyles.HexNumber);
                strPointer -= 2;
            }
            if (strPointer == -1)
                bytes[i] = byte.Parse(hexStr[..1], System.Globalization.NumberStyles.HexNumber);

            if (!littleEndian)
                Array.Reverse(bytes);

            return bytes;
        }
    }
}