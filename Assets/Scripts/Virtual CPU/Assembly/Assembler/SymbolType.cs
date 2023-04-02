using System;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    [Flags]
    public enum SymbolType
    {
        Unknown = 0,
        Label = 1 << 0,
        Constant = 1 << 1,
        
        Any = ~Unknown,
    }
}