using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Product : MathObject
    {
        public readonly ImmutableList<MathObject> elts;

        public Product(params MathObject[] ls) => elts = ImmutableList.Create(ls);

        public static Product FromRange(IEnumerable<MathObject> ls) => new Product(ls.ToArray());

        public override string FullForm() =>
            string.Join(" * ", elts.ConvertAll(elt => elt.Precedence() < Precedence() ? $"({elt})" : $"{elt}"));

        public override string StandardForm()
        {
            if (this.Denominator() == 1)
            {
                if (this.Const() < 0 && this / this.Const() is Sum) return $"-({this * -1})";

                if (this.Const() < 0) return $"-{this * -1}";

                return string.Join(" * ",
                    elts.ConvertAll(elt => elt.Precedence() < Precedence() || (elt is Power && (elt as Power).exp != new Integer(1) / 2) ? $"({elt})" : $"{elt}"));
            }

            var expr_a = this.Numerator();
            var expr_b = this.Denominator();

            var expr_a_ = expr_a is Sum || (expr_a is Power && (expr_a as Power).exp != new Integer(1) / 2) ? $"({expr_a})" : $"{expr_a}";

            var expr_b_ = expr_b is Sum || expr_b is Product || (expr_b is Power && (expr_b as Power).exp != new Integer(1) / 2) ? $"({expr_b})" : $"{expr_b}";

            return $"{expr_a_} / {expr_b_}";
        }

        public override int GetHashCode() => elts.GetHashCode();

        public override bool Equals(object obj) =>
            obj is Product && ListUtils.equal(elts, (obj as Product).elts);

        static ImmutableList<MathObject> MergeProducts(ImmutableList<MathObject> pElts, ImmutableList<MathObject> qElts)
        {
            if (pElts.Count == 0) return qElts;
            if (qElts.Count == 0) return pElts;

            var p = pElts[0];
            var ps = pElts.Cdr();

            var q = qElts[0];
            var qs = qElts.Cdr();

            var res = RecursiveSimplify(ListConstructor.ImList(p, q));

            if (res.Count == 0) return MergeProducts(ps, qs);

            if (res.Count == 1) return MergeProducts(ps, qs).Cons(res[0]);

            if (ListUtils.equal(res, ListConstructor.ImList(p, q))) return MergeProducts(ps, qElts).Cons(p);

            if (ListUtils.equal(res, ListConstructor.ImList(q, p))) return MergeProducts(pElts, qs).Cons(q);

            throw new Exception();
        }

        static ImmutableList<MathObject> SimplifyDoubleNumberProduct(DoubleFloat a, Number b)
        {
            double val = 0.0;

            if (b is DoubleFloat) val = a.val * ((DoubleFloat)b).val;

            if (b is Integer) val = a.val * (double)((Integer)b).val;

            if (b is Fraction) val = a.val * ((Fraction)b).ToDouble().val;

            if (val == 1.0) return ImmutableList.Create<MathObject>();

            return ListConstructor.ImList<MathObject>(new DoubleFloat(val));
        }

        public static ImmutableList<MathObject> RecursiveSimplify(ImmutableList<MathObject> elts)
        {
            if (elts.Count == 2)
            {
                if (elts[0] is Product && elts[1] is Product)
                    return MergeProducts(
                        ((Product)elts[0]).elts,
                        ((Product)elts[1]).elts);

                if (elts[0] is Product) return MergeProducts(((Product)elts[0]).elts, ListConstructor.ImList(elts[1]));

                if (elts[1] is Product) return MergeProducts(ListConstructor.ImList(elts[0]), ((Product)elts[1]).elts);

                //////////////////////////////////////////////////////////////////////

                if (elts[0] is DoubleFloat && elts[1] is Number)
                    return SimplifyDoubleNumberProduct((DoubleFloat)elts[0], (Number)elts[1]);

                if (elts[0] is Number && elts[1] is DoubleFloat)
                    return SimplifyDoubleNumberProduct((DoubleFloat)elts[1], (Number)elts[0]);

                //////////////////////////////////////////////////////////////////////

                if ((elts[0] is Integer || elts[0] is Fraction)
                    &&
                    (elts[1] is Integer || elts[1] is Fraction))
                {
                    var P = Rational.SimplifyRNE(new Product(elts[0], elts[1]));

                    if (P == 1) return ImmutableList.Create<MathObject>();

                    return ListConstructor.ImList(P);
                }

                if (elts[0] == 1) return ListConstructor.ImList(elts[1]);
                if (elts[1] == 1) return ListConstructor.ImList(elts[0]);

                var p = elts[0];
                var q = elts[1];

                if (OrderRelation.Base(p) == OrderRelation.Base(q))
                {
                    var res = OrderRelation.Base(p) ^ (OrderRelation.Exponent(p) + OrderRelation.Exponent(q));

                    if (res == 1) return ImmutableList.Create<MathObject>();

                    return ListConstructor.ImList(res);
                }

                if (OrderRelation.Compare(q, p)) return ListConstructor.ImList(q, p);

                return ListConstructor.ImList(p, q);
            }

            if (elts[0] is Product)
                return
                    MergeProducts(
                        ((Product)elts[0]).elts,
                        RecursiveSimplify(elts.Cdr()));

            return MergeProducts(
                ListConstructor.ImList(elts[0]),
                RecursiveSimplify(elts.Cdr()));

            throw new Exception();
        }

        public MathObject Simplify()
        {
            if (elts.Count == 1) return elts[0];

            if (elts.Any(elt => elt == 0)) return 0;

            var res = RecursiveSimplify(elts);

            if (res.IsEmpty) return 1;

            if (res.Count == 1) return res[0];

            // Without the below, the following throws an exception:
            // sqrt(a * b) * (sqrt(a * b) / a) / c

            if (res.Any(elt => elt is Product)) return Product.FromRange(res).Simplify();

            return Product.FromRange(res);
        }

        public override MathObject Numerator() =>
            Product.FromRange(elts.Select(elt => elt.Numerator())).Simplify();

        public override MathObject Denominator() =>
            Product.FromRange(elts.Select(elt => elt.Denominator())).Simplify();

        public MathObject Map(Func<MathObject, MathObject> proc) =>
            Product.FromRange(elts.Select(proc)).Simplify();
    }
}
