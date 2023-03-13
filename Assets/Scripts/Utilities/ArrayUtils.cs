using System.Collections.Generic;
using System.Linq;

namespace NineEightOhThree.Utilities
{
    public static class ArrayUtils
    {
        public static IList<T> ReplaceRange<T>(this IList<T> collection, int index, IList<T> data)
        {
            collection.RemoveAt(index);
            foreach (T elem in data.Reverse())
            {
                collection.Insert(index, elem);
            }

            return collection;
        }
    }
}