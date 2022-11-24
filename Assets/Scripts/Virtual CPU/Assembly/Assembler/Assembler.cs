using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            StringBuilder builder = new StringBuilder();
            foreach (Token token in tokens)
                builder.Append("(").Append(token.ToString()).Append(") ");
            Debug.Log(builder.ToString());

            return machineCode;
        }
    }
}