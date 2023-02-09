using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineEightOhThree.Utilities
{
    public static class SamUtils
    {
        private static readonly Regex WordPattern = new(@"1st|2nd|3rd|\dth|\.(?=\d)|\d|[a-zA-Z]+|[@!\\/_=*()"".+#$%&]");

        public static string CleanInput(string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }
        
        public static IEnumerable<string> SplitWords(string input)
        {
            MatchCollection wordMatches = WordPattern.Matches(input);
            return wordMatches.Select(match => input.Substring(match.Index, match.Length));
        }

        public static IEnumerable<int> GetWordIndexes(string input)
        {
            MatchCollection wordMatches = WordPattern.Matches(input);
            return wordMatches.Select(match => match.Index);
        }
    }
}