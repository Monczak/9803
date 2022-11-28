using System;

namespace NineEightOhThree.VirtualCPU
{
    [Flags]
    public enum AddressingMode
    {
        Implied = 1 << 0,
        Immediate = 1 << 1,
        ZeroPage = 1 << 2,
        ZeroPageX = 1 << 3,
        ZeroPageY = 1 << 4,
        Absolute = 1 << 5,
        AbsoluteX = 1 << 6,
        AbsoluteY = 1 << 7,
        Indirect = 1 << 8,
        IndexedIndirect = 1 << 9,
        IndirectIndexed = 1 << 10,
        Relative = 1 << 11,
        Accumulator = 1 << 12,
    }
}