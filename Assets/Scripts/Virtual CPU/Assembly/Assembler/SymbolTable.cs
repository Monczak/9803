using System;
using System.Collections.Generic;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> symbols;

        private readonly Dictionary<string, SymbolTable> childTables;

        private readonly string resourceLocation;

        public SymbolTable(string resourceLocation)
        {
            symbols = new Dictionary<string, Symbol>();
            childTables = new Dictionary<string, SymbolTable>();

            this.resourceLocation = resourceLocation;
        }

        private (string moduleName, string rest) SplitName(string name)
        {
            int firstDotIndex = name.IndexOf(".", StringComparison.Ordinal);
            string moduleName = name[..firstDotIndex];
            string rest = name[(firstDotIndex + 1)..];
            return (moduleName, rest);
        }

        public void Add(string name, Symbol symbol)
        {
            var split = SplitName(name);
            if (split.moduleName != string.Empty)
            {
                if (!childTables.ContainsKey(split.moduleName))
                    childTables.Add(split.moduleName, new SymbolTable(resourceLocation));   // TODO: Will this work? Should the resource location be inferred from the symbol?
                childTables[split.moduleName].Add(split.rest, symbol);
            }
            else
                symbols[name] = symbol;
        }

        public bool Contains(string name)
        {
            var split = SplitName(name);
            if (split.moduleName != string.Empty)
            {
                if (childTables.ContainsKey(split.moduleName))
                    return childTables[split.moduleName].Contains(split.rest);
            }
            return symbols.ContainsKey(split.rest);
        }

        // TODO: Figure out how to support multiple files with the same symbols
        // TODO: This will very likely break with multiple types of symbols if they have the same name
        // - disallow this or store all symbols of the same name in a dict
        public Symbol Find(string name, SymbolType typeMask = SymbolType.Any)
        {
            if (!Contains(name)) return null;

            if (typeMask != SymbolType.Any && symbols[name].Is(typeMask)) return null;

            return symbols[name];
        }
    }
}