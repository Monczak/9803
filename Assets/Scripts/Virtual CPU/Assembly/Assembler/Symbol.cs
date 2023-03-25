namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public abstract class Symbol
    {
        public string Name { get; protected set; }
        public string Location { get; protected set; }

        public bool IsDeclared { get; set; }
    }
    
    public abstract class Symbol<T> : Symbol
    {
        public T Value { get; set; }
    }
}