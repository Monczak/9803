namespace NineEightOhThree.VirtualCPU.Assembly
{
    public readonly struct Token
    {
        public TokenType Type { get; init; }
        public string Content { get; init; }
        public object Literal { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }

        public override string ToString()
        {
            return $"{Type.ToString()} {(Type == TokenType.Newline ? "" : Content)} L{Line} C{Column}";
        }
    }
}