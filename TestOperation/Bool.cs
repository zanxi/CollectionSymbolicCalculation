using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Bool : MathObject
    {
        public readonly bool val;

        public Bool(bool b) { val = b; }

        public override string FullForm() => val.ToString();

        public override bool Equals(object obj) => val == (obj as Bool)?.val;

        public override int GetHashCode() => val.GetHashCode();
    }
}
