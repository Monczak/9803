namespace NineEightOhThree.VirtualCPU.Assembly
{
    public class Token
    {
        public TokenType Type { get; init; }
        public TokenMetaType MetaType { get; set; }
        public string Content { get; init; }
        public object Literal { get; init; }
        public int Line { get; init; }
        public int Column { get; init; }
        public int CharIndex { get; init; }
        public string ResourceLocation { get; init; }
        
        public Token Previous { get; init; }

        public void SetMetaType(TokenMetaType metaType)
        {
            MetaType = metaType;
        }

        public override string ToString()
        {
            return $"{ResourceLocation.Split("/")[^1]} {Type.ToString()} {(Type == TokenType.Newline ? "" : Content)} L{Line} C{Column}";
        }
    }
}