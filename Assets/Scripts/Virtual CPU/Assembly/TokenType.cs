using System;

namespace NineEightOhThree.VirtualCPU.Assembly
{
    [Flags]
    public enum TokenType
    {
        Identifier = 1 << 0,
        Number = 1 << 1,
        LeftParen = 1 << 2,
        RightParen = 1 << 3,
        ImmediateOp = 1 << 4,
        Comma = 1 << 5,
        LabelDecl = 1 << 6,
        RegisterA = 1 << 7,
        RegisterX = 1 << 8,
        RegisterY = 1 << 9,
        Newline = 1 << 10,
        EndOfFile = 1 << 11
    }
}
