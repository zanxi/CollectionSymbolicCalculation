using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Fraction : Number
    {
        public readonly Integer numerator;
        public readonly Integer denominator;

        public Fraction(Integer a, Integer b)
        { numerator = a; denominator = b; }

        public override string FullForm() => numerator + "/" + denominator;

        public override DoubleFloat ToDouble() => new DoubleFloat((double)numerator.val / (double)denominator.val);
        //////////////////////////////////////////////////////////////////////

        public override bool Equals(object obj) =>
            numerator == (obj as Fraction)?.numerator
            &&
            denominator == (obj as Fraction)?.denominator;

        public override int GetHashCode() => new { numerator, denominator }.GetHashCode();

        public override MathObject Numerator() => numerator;

        public override MathObject Denominator() => denominator;
    }
}
