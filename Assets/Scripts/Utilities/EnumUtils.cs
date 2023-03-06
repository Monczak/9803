using System;
using System.Collections.Generic;
using System.Linq;

namespace NineEightOhThree.Utilities
{
    public static class EnumUtils
    {
        public static List<T> DeconstructFlags<T>(T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(flag => flags.HasFlag(flag)).ToList();
        } 
    }
}