using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Power : MathObject
    {
        public readonly MathObject bas;
        public readonly MathObject exp;

        public Power(MathObject a, MathObject b) { bas = a; exp = b; }

        public override string FullForm() =>
            string.Format("{0} ^ {1}",
                bas.Precedence() < Precedence() ? $"({bas})" : $"{bas}",
                exp.Precedence() < Precedence() ? $"({exp})" : $"{exp}");

        public override string StandardForm()
        {
            // x ^ 1/2   ->   sqrt(x)

            if (exp == new Integer(1) / new Integer(2)) return $"sqrt({bas})";

            return string.Format("{0} ^ {1}",
                bas.Precedence() < Precedence() ? $"({bas})" : $"{bas}",
                exp.Precedence() < Precedence() ? $"({exp})" : $"{exp}");
        }

        public override bool Equals(object obj) =>
            obj is Power && bas == (obj as Power).bas && exp == (obj as Power).exp;

        public MathObject Simplify()
        {
            var v = bas;
            var w = exp;

            if (v == 0) return 0;
            if (v == 1) return 1;
            if (w == 0) return 1;
            if (w == 1) return v;

            // Logic from MPL/Scheme:
            //
            //if (v is Integer && w is Integer)
            //    return
            //        new Integer(
            //            (int)Math.Pow(((Integer)v).val, ((Integer)w).val));

            // C# doesn't have built-in rationals. So:
            // 1 / 3 -> 3 ^ -1 -> 0.333... -> (int)... -> 0

            //if (v is Integer && w is Integer && ((Integer)w).val > 1)
            //    return
            //        new Integer(
            //            (int)Math.Pow(((Integer)v).val, ((Integer)w).val));

            var n = w;

            if ((v is Integer || v is Fraction) && n is Integer)
                return Rational.SimplifyRNE(new Power(v, n));

            if (v is DoubleFloat && w is Integer)
                return new DoubleFloat(Math.Pow(((DoubleFloat)v).val, (double)((Integer)w).val));

            if (v is DoubleFloat && w is Fraction)
                return new DoubleFloat(Math.Pow(((DoubleFloat)v).val, ((Fraction)w).ToDouble().val));

            if (v is Integer && w is DoubleFloat)
                return new DoubleFloat(Math.Pow((double)((Integer)v).val, ((DoubleFloat)w).val));

            if (v is Fraction && w is DoubleFloat)
                return new DoubleFloat(Math.Pow(((Fraction)v).ToDouble().val, ((DoubleFloat)w).val));

            if (v is Power && w is Integer)
            { return ((Power)v).bas ^ (((Power)v).exp * w); }

            if (v is Product && w is Integer)
                return (v as Product).Map(elt => elt ^ w);

            return new Power(v, w);
        }

        public override MathObject Numerator()
        {
            if (exp is Integer && exp < 0) return 1;

            if (exp is Fraction && exp < 0) return 1;

            return this;
        }

        public override MathObject Denominator()
        {
            if (exp is Integer && exp < 0) return this ^ -1;

            if (exp is Fraction && exp < 0) return this ^ -1;

            return 1;
        }

        public override int GetHashCode() => new { bas, exp }.GetHashCode();
    }
}
