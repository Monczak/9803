using System;

namespace NineEightOhThree.VirtualCPU.Assembly
{
    [Flags]
    public enum TokenMetaType
    {
        None = 0,
        Invalid = 1<<0,
        Instruction = 1<<1,
        Label = 1<<2,
        Directive = 1<<3,
        
        All = ~None,
        AllValid = All ^ Invalid,
    }
}