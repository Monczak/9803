using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NineEightOhThree.Utilities
{
    public static class StringUtils
    {
        public static string Beautify(string fieldName)
        {
            string[] strings = Regex.Split(fieldName, @"(?<!^)(?=[A-Z])");
            StringBuilder builder = new();
            foreach (string str in strings)
                builder.Append(str[0].ToString().ToUpper()).Append(str.AsSpan(1)).Append(" ");
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
    }
}