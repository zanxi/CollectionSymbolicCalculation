using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class And : Function
    {
        static MathObject AndProc(MathObject[] ls)
        {
            if (ls.Count() == 0) return true;

            if (ls.Count() == 1) return ls.First();

            if (ls.Any(elt => elt == false)) return false;

            if (ls.Any(elt => elt == true))
                return new And(ls.Where(elt => elt != true).ToArray()).Simplify();

            if (ls.Any(elt => elt is And))
            {
                var items = new List<MathObject>();

                foreach (var elt in ls)
                {
                    if (elt is And) items.AddRange((elt as And).args);

                    else items.Add(elt);
                }

                return And.FromRange(items).Simplify();
            }

            return new And(ls);
        }

        public And(params MathObject[] ls) : base("and", AndProc, ls) { }

        public And() : base("and", AndProc, new List<MathObject>()) { }

        public static And FromRange(IEnumerable<MathObject> ls) => new And(ls.ToArray());

        public MathObject Add(MathObject obj) =>
            And.FromRange(args.Add(obj)).Simplify();

        public MathObject AddRange(IEnumerable<MathObject> ls) =>
            And.FromRange(args.AddRange(ls)).Simplify();

        public MathObject Map(Func<MathObject, MathObject> proc) =>
            And.FromRange(args.Select(proc)).Simplify();
    }
}
