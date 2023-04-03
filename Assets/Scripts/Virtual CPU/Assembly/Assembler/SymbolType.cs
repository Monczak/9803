using System;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    [Flags]
    public enum SymbolType
    {
        Unknown = 1 << 0,
        Label = 1 << 1,
        Constant = 1 << 2,
        
        Any = ~Unknown,
    }
}