using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public static class ListConstructor
    {
        public static List<T> List<T>(params T[] items) => new List<T>(items);

        public static ImmutableList<T> ImList<T>(params T[] items) => ImmutableList.Create(items);
    }
}
