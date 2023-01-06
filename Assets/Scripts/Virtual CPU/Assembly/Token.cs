namespace NineEightOhThree.VirtualCPU.Assembly
{
    public struct Token
    {
        public TokenType Type { get; init; }
        public TokenMetaType MetaType { get; private set; }
        public string Content { get; init; }
        public object Literal { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }
        public int CharIndex { get; init; }

        public void SetMetaType(TokenMetaType metaType)
        {
            MetaType = metaType;
        }

        public override string ToString()
        {
            return $"{Type.ToString()} {(Type == TokenType.Newline ? "" : Content)} L{Line} C{Column}";
        }
    }
}