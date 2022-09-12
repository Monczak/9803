using System;

namespace NineEightOhThree.VirtualCPU.Interfacing
{
    [Serializable]
    public enum BindableType
    {
        Null,
        Byte,
        Ushort,
        Int,
        Long,
        Bool,
        Vector2,
        Vector2Int,
        Vector2Byte
    }
}
