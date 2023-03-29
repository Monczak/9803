using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> symbols;

        public SymbolTable()
        {
            symbols = new Dictionary<string, Symbol>();
        }

        public void Add(string name, Symbol symbol)
        {
            symbols[name] = symbol;
        }

        public bool Contains(string name)
        {
            return symbols.ContainsKey(name);
        }

        // TODO: Figure out how to support multiple files with the same symbols
        // TODO: This will very likely break with multiple types of symbols if they have the same name
        // - disallow this or store all symbols of the same name in a dict
        public T Find<T>(string name) where T : Symbol
        {
            return Contains(name) ? (T)symbols[name] : null;
        }

        public Symbol Find(string name)
        {
            return Find<Symbol>(name);
        }
    }
}