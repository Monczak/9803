using NineEightOhThree.Utilities;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class Symbol
    {
        public string Name { get; }
        public string Location { get; }

        public bool IsDeclared { get; set; }

        public Symbol(string name, string location, bool isDeclared)
        {
            Name = name;
            Location = location;
            IsDeclared = isDeclared;
        }
    }
    
    public class Symbol<T> : Symbol
    {
        public T Value { get; set; }

        public Symbol(string name, string location, bool isDeclared, T value) : base(name, location, isDeclared)
        {
            Value = value;
        }
    }
}