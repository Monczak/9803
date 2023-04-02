using NineEightOhThree.Utilities;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Symbol
    {
        public string Name { get; }
        public string Location { get; }

        public bool IsDeclared { get; set; }
        
        public ushort? Value { get; set; }
        
        public SymbolType Type { get; set; }

        public Symbol(SymbolType type, string name, string location, bool isDeclared, ushort? value = null)
        {
            Type = type;
            Name = name;
            Location = location;
            IsDeclared = isDeclared;
            Value = value;
        }

        public Symbol To(SymbolType type)
        {
            Type = type;
            return this;
        }

        public bool Is(SymbolType typeMask) => (Type & typeMask) != 0;
    }
}