using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Sum : MathObject
    {
        public readonly ImmutableList<MathObject> elts;

        public Sum(params MathObject[] ls) { elts = ImmutableList.Create(ls); }

        public static Sum FromRange(IEnumerable<MathObject> ls) => new Sum(ls.ToArray());

        public override int GetHashCode() => elts.GetHashCode();

        public override bool Equals(object obj) =>
            obj is Sum && ListUtils.equal(elts, (obj as Sum).elts);

        static ImmutableList<MathObject> MergeSums(ImmutableList<MathObject> pElts, ImmutableList<MathObject> qElts)
        {
            if (pElts.Count == 0) return qElts;
            if (qElts.Count == 0) return pElts;

            var p = pElts[0];
            var ps = pElts.Cdr();

            var q = qElts[0];
            var qs = qElts.Cdr();

            var res = RecursiveSimplify(ListConstructor.ImList(p, q));

            if (res.Count == 0) return MergeSums(ps, qs);

            if (res.Count == 1) return MergeSums(ps, qs).Cons(res[0]);

            if (ListUtils.equal(res, ListConstructor.ImList(p, q))) return MergeSums(ps, qElts).Cons(p);

            if (ListUtils.equal(res, ListConstructor.ImList(q, p))) return MergeSums(pElts, qs).Cons(q);

            throw new Exception();
        }

        static ImmutableList<MathObject> SimplifyDoubleNumberSum(DoubleFloat a, Number b)
        {
            double val = 0.0;

            if (b is DoubleFloat) val = a.val + ((DoubleFloat)b).val;

            if (b is Integer) val = a.val + (double)((Integer)b).val;

            if (b is Fraction) val = a.val + ((Fraction)b).ToDouble().val;

            if (val == 0.0) return ImmutableList.Create<MathObject>();

            return ImmutableList.Create<MathObject>(new DoubleFloat(val));
        }

        static ImmutableList<MathObject> RecursiveSimplify(ImmutableList<MathObject> elts)
        {
            if (elts.Count == 2)
            {
                if (elts[0] is Sum && elts[1] is Sum)
                    return MergeSums(
                        ((Sum)elts[0]).elts,
                        ((Sum)elts[1]).elts);

                if (elts[0] is Sum)
                    return MergeSums(
                        ((Sum)elts[0]).elts,
                        ListConstructor.ImList(elts[1]));

                if (elts[1] is Sum)
                    return MergeSums(
                        ListConstructor.ImList(elts[0]),
                        ((Sum)elts[1]).elts);

                //////////////////////////////////////////////////////////////////////

                if (elts[0] is DoubleFloat && elts[1] is Number)
                    return SimplifyDoubleNumberSum((DoubleFloat)elts[0], (Number)elts[1]);

                if (elts[0] is Number && elts[1] is DoubleFloat)
                    return SimplifyDoubleNumberSum((DoubleFloat)elts[1], (Number)elts[0]);

                //////////////////////////////////////////////////////////////////////

                if ((elts[0] is Integer || elts[0] is Fraction)
                    &&
                    (elts[1] is Integer || elts[1] is Fraction))
                {
                    var P = Rational.SimplifyRNE(new Sum(elts[0], elts[1]));

                    if (P == 0) return ImmutableList.Create<MathObject>();

                    return ListConstructor.ImList(P);
                }

                if (elts[0] == 0) return ListConstructor.ImList(elts[1]);

                if (elts[1] == 0) return ListConstructor.ImList(elts[0]);

                var p = elts[0];
                var q = elts[1];

                if (p.Term() == q.Term())
                {
                    var res = p.Term() * (p.Const() + q.Const());

                    if (res == 0) return ImmutableList.Create<MathObject>();

                    return ListConstructor.ImList(res);
                }

                if (OrderRelation.Compare(q, p)) return ListConstructor.ImList(q, p);

                return ListConstructor.ImList(p, q);
            }

            if (elts[0] is Sum)
                return
                    MergeSums(
                        ((Sum)elts[0]).elts, RecursiveSimplify(elts.Cdr()));

            return MergeSums(
                ListConstructor.ImList(elts[0]), RecursiveSimplify(elts.Cdr()));
        }

        public MathObject Simplify()
        {
            if (elts.Count == 1) return elts[0];

            var res = RecursiveSimplify(elts);

            if (res.Count == 0) return 0;
            if (res.Count == 1) return res[0];

            return Sum.FromRange(res);
        }

        public override string FullForm() =>
            String.Join(" + ", elts.ConvertAll(elt => elt.Precedence() < Precedence() ? $"({elt})" : $"{elt}"));

        public override string StandardForm()
        {
            var result = string.Join(" ",
                elts
                    .ConvertAll(elt =>
                    {
                        var elt_ = elt.Const() < 0 ? elt * -1 : elt;

                        var elt__ = elt.Const() < 0 && elt_ is Sum || (elt is Power && (elt as Power).exp != new Integer(1) / 2) ? $"({elt_})" : $"{elt_}";

                        return elt.Const() < 0 ? $"- {elt__}" : $"+ {elt__}";
                    }));

            if (result.StartsWith("+ ")) return result.Remove(0, 2); // "+ x + y"   ->   "x + y"

            if (result.StartsWith("- ")) return result.Remove(1, 1); // "- x + y"   ->   "-x + y"

            return result;
        }

        public MathObject Map(Func<MathObject, MathObject> proc) =>
            Sum.FromRange(elts.Select(proc)).Simplify();
    }
}
