using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Function : MathObject
    {
        public delegate MathObject Proc(params MathObject[] ls);

        public readonly String name;

        public readonly Proc proc;

        public readonly ImmutableList<MathObject> args;

        public Function(string name, Proc proc, IEnumerable<MathObject> args)
        {
            this.name = name;
            this.proc = proc;
            this.args = ImmutableList.CreateRange(args);
        }

        public override bool Equals(object obj) =>
            GetType() == obj.GetType() &&
            name == (obj as Function).name &&
            ListUtils.equal(args, ((Function)obj).args);

        public MathObject Simplify() => proc == null ? this : proc(args.ToArray());

        public override string FullForm() => $"{name}({string.Join(", ", args)})";

        public MathObject Clone() => MemberwiseClone() as MathObject;

        public override int GetHashCode() => new { name, args }.GetHashCode();
    }
}
