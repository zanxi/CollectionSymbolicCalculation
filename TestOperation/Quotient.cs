using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    class Quotient : MathObject
    {
        public readonly ImmutableList<MathObject> elts;

        public Quotient(params MathObject[] ls) => elts = ImmutableList.Create(ls);

        public MathObject Simplify() => elts[0] * (elts[1] ^ -1);
    }
}
