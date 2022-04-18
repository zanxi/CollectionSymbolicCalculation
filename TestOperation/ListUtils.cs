using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public static class ListUtils
    {
        public static ImmutableList<MathObject> Cons(this ImmutableList<MathObject> obj, MathObject elt) =>
            obj.Insert(0, elt);

        public static ImmutableList<MathObject> Cdr(this ImmutableList<MathObject> obj) => obj.RemoveAt(0);

        public static bool equal(ImmutableList<MathObject> a, ImmutableList<MathObject> b)
        {
            if (a.Count == 0 && b.Count == 0) return true;

            if (a.Count == 0) return false;

            if (b.Count == 0) return false;

            if (a[0] == b[0]) return equal(a.Cdr(), b.Cdr());

            return false;
        }
    }
}
