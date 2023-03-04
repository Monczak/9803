using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineEightOhThree.Utilities
{
    public static class SamUtils
    {
        private static readonly Regex WordPattern = new(@"\(?(?:1st|2nd|3rd|\dth|[.,!?](?=\d)|\d[.,!?]?(?!\d)|\d|[a-zA-Z'-]+[.,!?]*|[.,!?@\/_=*""+#$%&^])\)?");


        private static readonly Dictionary<string, string> CommandTranslations = new()
        {
            {"{pause}", "\x80"},
            {"{pause1}", "\x80"},
            {"{pause2}", "\x80\x80"},
            {"{pause3}", "\x80\x80\x80\x80"},
            {"{pause4}", "\x80\x80\x80\x80\x80\x80\x80\x80"},
        };

        public static string CleanInputForSam(string input)
        {
            string result = input;
            
            // Translate commands
            result = TranslateCommands(result);
            
            // Clean up multiple hyphens
            result = Regex.Replace(result, @"[-]+", " ");
            
            // Clean up multiple spaces
            result = Regex.Replace(result, @"\s+", " ");

            return result;
        }
        
        public static string CleanInputForDialogue(string input)
        {
            string result = input;
            
            // Clean up commands
            result = CommandTranslations.Aggregate(result, (current, pair) => current.Replace(pair.Key, ""));
            
            // Clean up multiple spaces
            result = Regex.Replace(result, @"\s+", " ");
            
            // Strip spaces from start and end
            result = result.TrimStart(' ').TrimEnd(' ');

            return result;
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

        public static string TranslateCommands(string input)
        {
            return CommandTranslations.Aggregate(input, (current, pair) => current.Replace(pair.Key, pair.Value));
        }
    }
}