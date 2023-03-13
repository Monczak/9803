using System.Collections.Generic;
using System.Linq;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public static class AssemblerCache
    {
        private static Parser.GrammarGraph grammarGraph;
        public static Parser.GrammarGraph GrammarGraph
        {
            get => grammarGraph;
            set => grammarGraph ??= value;
        }

        private static Dictionary<string, (string code, List<AbstractStatement> stmts)> parseCache;
        
        public static bool TryGetParseCache(string resourceLocation, string code, out List<AbstractStatement> stmts)
        {
            parseCache ??= new Dictionary<string, (string code, List<AbstractStatement> stmts)>();
            stmts = null;
            
            if (!parseCache.ContainsKey(resourceLocation)) return false;
            if (parseCache[resourceLocation].code != code) return false;
            
            stmts = new List<AbstractStatement>(parseCache[resourceLocation].stmts);
            return true;
        }

        public static void CacheParseResults(string resourceLocation, string code, List<AbstractStatement> stmts)
        {
            parseCache ??= new Dictionary<string, (string code, List<AbstractStatement> stmts)>();
            parseCache[resourceLocation] = (code, new List<AbstractStatement>(stmts));
        }
    }
}