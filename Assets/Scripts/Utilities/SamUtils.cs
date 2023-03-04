using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NineEightOhThree.Utilities
{
    public static class SamUtils
    {
        private static readonly Regex WordPattern = new(@"\(?(?:1st|2nd|3rd|\dth|[.,!?](?=\d)|\d[.,!?]?(?!\d)|\d|[a-zA-Z'-]+[.,!?]*|[.,!?@\/_=*""+#$%&^])\)?");


        private static readonly Dictionary<string, string> SamCommandTranslations = new()
        {
            {"{pause}", "\x80"},
            {"{pause1}", "\x80"},
            {"{pause2}", "\x80\x80"},
            {"{pause3}", "\x80\x80\x80\x80"},
            {"{pause4}", "\x80\x80\x80\x80\x80\x80\x80\x80"},
        };
        
        private static readonly Dictionary<string, string> TextCommandTranslations = new()
        {
            {"[newline]", "\n"},
        };

        public static string CleanInputForSam(string input)
        {
            string result = input;
            
            // Translate SAM commands and delete text commands
            result = TranslateSamCommands(result);
            result = TextCommandTranslations.Aggregate(result, (current, pair) => current.Replace(pair.Key, pair.Value));

            // Clean up multiple hyphens
            result = Regex.Replace(result, @"[-]+", " ");
            
            // Clean up multiple spaces
            result = Regex.Replace(result, @"\s+", " ");

            return result;
        }
        
        public static string CleanInputForDialogue(string input)
        {
            string result = input;
            
            // Clean up multiple spaces
            result = Regex.Replace(result, @"\s+", " ");
            
            // Delete SAM commands and translate text commands
            result = SamCommandTranslations.Aggregate(result, (current, pair) => current.Replace(pair.Key, ""));
            result = TranslateTextCommands(result);
            
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

        private static string TranslateSamCommands(string input)
        {
            return SamCommandTranslations.Aggregate(input, (current, pair) => current.Replace(pair.Key, pair.Value));
        }

        private static string TranslateTextCommands(string result)
        {
            return TextCommandTranslations.Aggregate(result, (current, pair) => current.Replace(pair.Key, pair.Value));
        }
    }
}