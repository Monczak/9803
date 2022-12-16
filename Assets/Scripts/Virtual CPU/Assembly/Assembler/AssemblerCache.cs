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
    }
}