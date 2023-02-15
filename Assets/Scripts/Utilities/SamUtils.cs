using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineEightOhThree.Utilities
{
    public static class SamUtils
    {
        // FIXME: Parentheses don't work properly (should be a part of a word), periods after numbers aren't part of the word
        private static readonly Regex WordPattern = new(@"1st|2nd|3rd|\dth|\.(?=\d)|\d|[a-zA-Z']+[.,!?]?|[.,!?@\/_=*()""+#$%&^]");

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
        
        public static IEnumerable<Match> GetWordMatches(string input)
        {
            MatchCollection wordMatches = WordPattern.Matches(input);
            return wordMatches;
        }
    }
}