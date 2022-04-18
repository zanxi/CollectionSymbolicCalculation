using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public static class OrderRelation
    {
        public static MathObject Base(MathObject u) => u is Power ? (u as Power).bas : u;

        public static MathObject Exponent(MathObject u) => u is Power ? (u as Power).exp : 1;

        public static MathObject Term(this MathObject u)
        {
            if (u is Product && ((Product)u).elts[0] is Number)
                return Product.FromRange((u as Product).elts.Cdr());
            // return (u as Product).Cdr()

            if (u is Product) return u;

            return new Product(u);
        }

        public static MathObject Const(this MathObject u) =>
            (u is Product && (u as Product).elts[0] is Number) ? (u as Product).elts[0] : 1;

        public static bool O3(ImmutableList<MathObject> uElts, ImmutableList<MathObject> vElts)
        {
            if (uElts.IsEmpty) return true;
            if (vElts.IsEmpty) return false;

            var u = uElts.First();
            var v = vElts.First();

            return (!(u == v)) ?
                Compare(u, v) :
                O3(uElts.Cdr(), vElts.Cdr());
        }

        public static bool Compare(MathObject u, MathObject v)
        {
            if (u is DoubleFloat && v is DoubleFloat) return ((DoubleFloat)u).val < ((DoubleFloat)v).val;

            // if (u is DoubleFloat && v is Integer) return ((DoubleFloat)u).val < ((Integer)v).val;

            if (u is DoubleFloat && v is Integer) return ((DoubleFloat)u).val < ((double)((Integer)v).val);

            if (u is DoubleFloat && v is Fraction) return
                ((DoubleFloat)u).val < ((double)((Fraction)v).numerator.val) / ((double)((Fraction)v).denominator.val);

            if (u is Integer && v is DoubleFloat) return ((double)((Integer)u).val) < ((DoubleFloat)v).val;

            if (u is Fraction && v is DoubleFloat) return
                ((double)((Fraction)u).numerator.val) / ((double)((Fraction)u).denominator.val) < ((DoubleFloat)v).val;

            if (u is Integer)
                return Compare(new Fraction((Integer)u, new Integer(1)), v);

            if (v is Integer)
                return Compare(u, new Fraction((Integer)v, new Integer(1)));

            if (u is Fraction && v is Fraction)
            {
                var u_ = (Fraction)u;
                var v_ = (Fraction)v;

                // a / b   <   c / d
                //
                // (a d) / (b d)   <   (c b) / (b d)

                return
                    (u_.numerator.val * v_.denominator.val)
                    <
                    (v_.numerator.val * u_.denominator.val);
            }

            if (u is Symbol && v is Symbol)
                return
                    String.Compare(
                        ((Symbol)u).name,
                        ((Symbol)v).name) < 0;

            if (u is Product && v is Product)
                return O3(
                    (u as Product).elts.Reverse(),
                    (v as Product).elts.Reverse());

            if (u is Sum && v is Sum)
                return O3(
                    (u as Sum).elts.Reverse(),
                    (v as Sum).elts.Reverse());

            if (u is Power && v is Power)
            {
                var u_ = (Power)u;
                var v_ = (Power)v;

                return (u_.bas == v_.bas) ?
                    Compare(u_.exp, v_.exp) :
                    Compare(u_.bas, v_.bas);
            }

            if (u is Function && v is Function)
            {
                var u_ = (Function)u;
                var v_ = (Function)v;

                return u_.name == v_.name ?
                    O3(u_.args, v_.args) :
                    String.Compare(u_.name, v_.name) < 0;
            }

            if (u is Number && !(v is Number)) return true;

            if (u is Product &&
                (v is Power || v is Sum || v is Function || v is Symbol))
                return Compare(u, new Product(v));

            if (u is Power && (v is Sum || v is Function || v is Symbol))
                return Compare(u, new Power(v, new Integer(1)));

            if (u is Sum && (v is Function || v is Symbol))
                return Compare(u, new Sum(v));

            if (u is Function && v is Symbol)
            {
                var u_ = (Function)u;
                var v_ = (Symbol)v;

                return u_.name == v_.name ?
                    false :
                    Compare(new Symbol(u_.name), v);
            }

            return !Compare(v, u);
        }
    }
}
