namespace NineEightOhThree.VirtualCPU.Assembly
{
    public enum TokenType
    {
        Identifier,
        Number,
        LeftParen,
        RightParen,
        Comment,
        HexOp,
        BinaryOp,
        ImmediateOp,
        Comma,
        Label,
        EndOfFile
    }
}
