using NineEightOhThree.Utilities;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Symbol
    {
        public string Name { get; }
        public string Location { get; }
        public string Namespace { get; }

        public string SimpleName => Name.Split(".")[^1];
        public string NamespacedName => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";

        public bool IsDeclared { get; set; }
        
        public ushort? Value { get; set; }
        
        public SymbolType Type { get; private set; }
        
        public Token Token { get; }

        public Symbol(SymbolType type, string name, string location, string @namespace, bool isDeclared, Token token, ushort? value = null)
        {
            Type = type;
            Name = name;
            Location = location;
            Namespace = @namespace;
            IsDeclared = isDeclared;
            Value = value;
            Token = token;
        }

        public Symbol To(SymbolType type)
        {
            Type = type;
            return this;
        }

        public bool Is(SymbolType typeMask) => (Type & typeMask) != 0;

        public override string ToString()
        {
            return $"{NamespacedName}: {(Value is null ? "(null)" : Value)}";
        }
    }
}