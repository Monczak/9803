using System;
using System.Collections.Generic;
using System.Linq;

namespace NineEightOhThree.Utilities
{
    public static class TypeUtils
    {
        public static IEnumerable<Type> TypeHierarchy(this Type type)
        {
            do
            {
                yield return type;
                type = type.BaseType;
            } while (type != null);
        }
        
        public static Type MostDerivedCommonBase(IEnumerable<Type> types)
        {
            IEnumerable<Type> enumerable = types as Type[] ?? types.ToArray();
            if (!enumerable.Any()) return null;

            Dictionary<Type, int> counts = enumerable.SelectMany(t => t.TypeHierarchy())
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            var total = counts[typeof(object)];
            return enumerable.First().TypeHierarchy().First(t => counts[t] == total);
        }
    }
}