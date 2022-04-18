using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    class Difference : MathObject
    {
        public readonly ImmutableList<MathObject> elts;

        public Difference(params MathObject[] ls) => elts = ImmutableList.Create(ls);

        public MathObject Simplify()
        {
            if (elts.Count == 1) return -1 * elts[0];

            if (elts.Count == 2) return elts[0] + -1 * elts[1];

            throw new Exception();
        }
    }
}
