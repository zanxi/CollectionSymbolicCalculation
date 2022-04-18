using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class DoubleFloat : Number
    {
        public static double? tolerance;

        public readonly double val;

        public DoubleFloat(double n) { val = n; }

        public override string FullForm() => val.ToString("R");

        //public bool EqualWithinTolerance(DoubleFloat obj)
        //{
        //    if (tolerance.HasValue)
        //        return Math.Abs(val - obj.val) < tolerance;

        //    throw new Exception();
        //}

        public override bool Equals(object obj)
        {
            if (obj is DoubleFloat && tolerance.HasValue)
                return Math.Abs(val - (obj as DoubleFloat).val) < tolerance;

            if (obj is DoubleFloat) return val == ((DoubleFloat)obj).val;

            return false;
        }

        public override int GetHashCode() => val.GetHashCode();

        public override DoubleFloat ToDouble() => this;
    }    
}
