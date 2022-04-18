using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;
    
        public static class Rational
        {
            static BigInteger Div(BigInteger a, BigInteger b)
            { BigInteger rem; return BigInteger.DivRem(a, b, out rem); }

            static BigInteger Rem(BigInteger a, BigInteger b)
            { BigInteger rem; BigInteger.DivRem(a, b, out rem); return rem; }

            static BigInteger Gcd(BigInteger a, BigInteger b)
            {
                BigInteger r;
                while (b != 0)
                {
                    r = Rem(a, b);
                    a = b;
                    b = r;
                }
                return BigInteger.Abs(a);
            }

            public static MathObject SimplifyRationalNumber(MathObject u)
            {
                if (u is Integer) return u;

                if (u is Fraction)
                {
                    var u_ = (Fraction)u;
                    var n = u_.numerator.val;
                    var d = u_.denominator.val;

                    if (Rem(n, d) == 0) return Div(n, d);

                    var g = Gcd(n, d);

                    if (d > 0) return new Fraction(Div(n, g), Div(d, g));

                    if (d < 0) return new Fraction(Div(-n, g), Div(-d, g));
                }

                throw new Exception();
            }

            public static Integer Numerator(MathObject u)
            {
                // (a / b) / (c / d)
                // (a / b) * (d / c)
                // (a * d) / (b * c)

                if (u is Integer) return (Integer)u;

                if (u is Fraction)
                {
                    var u_ = u as Fraction;

                    //return
                    //    Numerator(u_.numerator).val
                    //    *
                    //    Denominator(u_.denominator).val;

                    return
                        Numerator(u_.numerator)
                        *
                        Denominator(u_.denominator);
                }

                throw new Exception();
            }

            public static Integer Denominator(MathObject u)
            {
                // (a / b) / (c / d)
                // (a / b) * (d / c)
                // (a * d) / (b * c)

                if (u is Integer) return new Integer(1);

                if (u is Fraction)
                {
                    var u_ = u as Fraction;

                    return
                        Denominator(u_.numerator)
                        *
                        Numerator(u_.denominator);
                }

                throw new Exception();
            }

            public static Fraction EvaluateSum(MathObject v, MathObject w) =>

                // a / b + c / d
                // a d / b d + c b / b d
                // (a d + c b) / (b d)

                new Fraction(
                    Numerator(v) * Denominator(w) + Numerator(w) * Denominator(v),
                    Denominator(v) * Denominator(w));

            public static Fraction EvaluateDifference(MathObject v, MathObject w) =>
                new Fraction(
                    Numerator(v) * Denominator(w) - Numerator(w) * Denominator(v),
                    Denominator(v) * Denominator(w));

            public static Fraction EvaluateProduct(MathObject v, MathObject w) =>
                new Fraction(
                    Numerator(v) * Numerator(w),
                    Denominator(v) * Denominator(w));

            public static MathObject EvaluateQuotient(MathObject v, MathObject w)
            {
                if (Numerator(w).val == 0) return new Undefined();

                return
                    new Fraction(
                        Numerator(v) * Denominator(w),
                        Numerator(w) * Denominator(v));
            }

            public static MathObject EvaluatePower(MathObject v, BigInteger n)
            {
                if (Numerator(v).val != 0)
                {
                    if (n > 0) return EvaluateProduct(EvaluatePower(v, n - 1), v);

                    if (n == 0) return 1;

                    if (n == -1) return new Fraction(Denominator(v), Numerator(v));

                    if (n < -1)
                    {
                        var s = new Fraction(Denominator(v), Numerator(v));

                        return EvaluatePower(s, -n);
                    }
                }

                if (n >= 1) return 0;
                if (n <= 0) return new Undefined();

                throw new Exception();
            }

            public static MathObject SimplifyRNERec(MathObject u)
            {
                if (u is Integer) return u;

                if (u is Fraction)
                    if (Denominator((Fraction)u).val == 0) return new Undefined();
                    else return u;

                if (u is Sum && ((Sum)u).elts.Count == 1)
                { return SimplifyRNERec(((Sum)u).elts[0]); }

                if (u is Difference && ((Difference)u).elts.Count == 1)
                {
                    var v = SimplifyRNERec(((Difference)u).elts[0]);

                    if (v == new Undefined()) return v;

                    return EvaluateProduct(-1, v);
                }

                if (u is Sum && ((Sum)u).elts.Count == 2)
                {
                    var v = SimplifyRNERec(((Sum)u).elts[0]);
                    var w = SimplifyRNERec(((Sum)u).elts[1]);

                    if (v == new Undefined() || w == new Undefined())
                        return new Undefined();

                    return EvaluateSum(v, w);
                }

                if (u is Product && ((Product)u).elts.Count == 2)
                {
                    var v = SimplifyRNERec(((Product)u).elts[0]);
                    var w = SimplifyRNERec(((Product)u).elts[1]);

                    if (v == new Undefined() || w == new Undefined())
                        return new Undefined();

                    return EvaluateProduct(v, w);
                }

                if (u is Difference && ((Difference)u).elts.Count == 2)
                {
                    var v = SimplifyRNERec(((Difference)u).elts[0]);
                    var w = SimplifyRNERec(((Difference)u).elts[1]);

                    if (v == new Undefined() || w == new Undefined())
                        return new Undefined();

                    return EvaluateDifference(v, w);
                }

                if (u is Fraction)
                {
                    var v = SimplifyRNERec(((Fraction)u).numerator);
                    var w = SimplifyRNERec(((Fraction)u).denominator);

                    if (v == new Undefined() || w == new Undefined())
                        return new Undefined();

                    return EvaluateQuotient(v, w);
                }

                if (u is Power)
                {
                    var v = SimplifyRNERec(((Power)u).bas);

                    if (v == new Undefined()) return v;

                    return EvaluatePower(v, ((Integer)((Power)u).exp).val);
                }

                throw new Exception();
            }

            public static MathObject SimplifyRNE(MathObject u)
            {
                var v = SimplifyRNERec(u);
                if (v is Undefined) return v;
                return SimplifyRationalNumber(v);
            }
        }    
}
