using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class Assembler
    {
        public static List<byte> Assemble(string input)
        {
            List<byte> machineCode = new();

            var tokens = Lexer.Lex(input);
            foreach (Token token in tokens)
                Debug.Log(token);

            return machineCode;
        }
    }
}