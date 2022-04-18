using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Symbol : MathObject
    {
        public readonly String name;

        public Symbol(String str) { name = str; }

        public override string FullForm() => name;

        public override int GetHashCode() => name.GetHashCode();

        public override bool Equals(Object obj) =>
            obj is Symbol ? name == (obj as Symbol).name : false;
    }
}
