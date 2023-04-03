using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class SymbolTable
    {
        private readonly Dictionary<string, Symbol> symbols;
        private readonly HashSet<Symbol> unresolvedSymbols;

        private readonly Dictionary<string, SymbolTable> subTables;

        private readonly string @namespace;

        public SymbolTable(string @namespace)
        {
            symbols = new Dictionary<string, Symbol>();
            subTables = new Dictionary<string, SymbolTable>();

            unresolvedSymbols = new HashSet<Symbol>();

            this.@namespace = @namespace;
        }

        private Symbol this[string name]
        {
            get => symbols[name];
            set => symbols[name] = value;
        }
        
        private static (string firstNamespace, string rest) SplitName(string theNamespace)
        {
            int firstDotIndex = theNamespace.IndexOf(".", StringComparison.Ordinal);
            string firstNamespace = firstDotIndex == -1 ? theNamespace : theNamespace[..firstDotIndex];
            string rest = firstDotIndex == -1 ? "" : theNamespace[(firstDotIndex + 1)..];
            return (firstNamespace, rest);
        }

        private OperationResult AddRecursively(string theNamespace, Symbol symbol, SymbolTable subTable)
        {
            while (true)
            {
                var (firstNamespace, rest) = SplitName(theNamespace);

                if (string.IsNullOrEmpty(firstNamespace))
                {
                    if (subTable.symbols.ContainsKey(symbol.SimpleName))
                        return OperationResult.Error(SyntaxErrors.SymbolAlreadyDeclared(symbol.Token));
                    subTable[symbol.SimpleName] = symbol;
                }
                else
                {
                    if (!subTables.ContainsKey(firstNamespace))
                    {
                        // Debug.Log($"Symbol table for namespace {firstNamespace} does not exist, creating");
                        subTables.Add(firstNamespace, new SymbolTable(firstNamespace));
                    }

                    theNamespace = rest;
                    subTable = subTables[firstNamespace];
                    continue;
                }

                break;
            }

            return OperationResult.Success();
        }

        public OperationResult Add(Symbol symbol)
        {
            if (symbol.Is(SymbolType.Unknown))
            {
                // Debug.Log($"Adding unresolved symbol {symbol.Name} from {symbol.Location} in {(symbol.Namespace == string.Empty ? "(root)" : symbol.Namespace)}");
                unresolvedSymbols.Add(symbol);
                return OperationResult.Success();
            }

            // Debug.Log($"Adding symbol {symbol.Name} from {symbol.Location} in {(symbol.Namespace == string.Empty ? "(root)" : symbol.Namespace)}");
            return AddRecursively(symbol.Namespace, symbol, this);
        }

        public bool Contains(string symbolName)
        {
            return FindRecursively(symbolName, this) is not null;
        }

        private Symbol FindRecursively(string namespacedName, SymbolTable subTable)
        {
            while (true)
            {
                var (firstNamespace, rest) = SplitName(namespacedName);

                if (string.IsNullOrEmpty(rest))
                {
                    return subTable.symbols.ContainsKey(namespacedName) ? subTable.symbols[namespacedName] : null;
                }

                if (subTables.ContainsKey(firstNamespace))
                {
                    namespacedName = rest;
                    subTable = subTables[firstNamespace];
                }
                else
                {
                    return null;
                }
            }
        }
        
        // TODO: Support multiple types of symbols with the same name
        public Symbol Find(string symbolName, string theNamespace, SymbolType typeMask = SymbolType.Any)
        {
            // Debug.Log($"Finding {symbolName}");
            
            Symbol foundSymbol = FindRecursively(symbolName, string.IsNullOrEmpty(theNamespace) ? this : subTables[theNamespace]);
            if (foundSymbol is null) return null;
            if (!foundSymbol.Is(typeMask)) return null;

            // Debug.Log($"Found {symbolName}");
            return foundSymbol;
        }

        public IEnumerable<OperationResult> FindUsesOfUndeclaredSymbols()
        {
            foreach (Symbol symbol in unresolvedSymbols)
            {
                Symbol corrSymbol = Find(symbol.NamespacedName, @namespace);
                if (corrSymbol is null)
                {
                    yield return OperationResult.Error(SyntaxErrors.UseOfUndeclaredSymbol(symbol.Token, symbol));
                    continue;
                }

                corrSymbol.IsDeclared = true;
            }
        }

        public List<Symbol> GetAllSymbols()
        {
            void AddSymbols(List<Symbol> list, SymbolTable table)
            {
                list.AddRange(table.symbols.Values);
                foreach (SymbolTable t in table.subTables.Values)
                {
                    AddSymbols(list, t);
                }
            }
            
            List<Symbol> result = new();
            AddSymbols(result, this);
            return result;
        }
    }
}