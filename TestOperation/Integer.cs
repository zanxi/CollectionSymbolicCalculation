using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Integer : Number
    {
        public readonly BigInteger val;

        public Integer(int n) { val = n; }

        public Integer(BigInteger n) { val = n; }

        public static implicit operator Integer(BigInteger n) => new Integer(n);

        // public static MathObject operator *(MathObject a, MathObject b) => new Product(a, b).Simplify();

        public static Integer operator +(Integer a, Integer b) => a.val + b.val;
        public static Integer operator -(Integer a, Integer b) => a.val - b.val;
        public static Integer operator *(Integer a, Integer b) => a.val * b.val;

        public override string FullForm() => val.ToString();

        public override bool Equals(object obj) => val == (obj as Integer)?.val;

        public override int GetHashCode() => val.GetHashCode();

        public override DoubleFloat ToDouble() => new DoubleFloat((double)val);
    }    
}
