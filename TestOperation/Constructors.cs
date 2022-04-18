using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public static class Constructors
    {
        public static MathObject sin(MathObject obj) => new Sin(obj).Simplify();
        public static MathObject cos(MathObject obj) => new Cos(obj).Simplify();
        public static MathObject tan(MathObject obj) => new Tan(obj).Simplify();

        public static MathObject asin(MathObject obj) => new Asin(obj).Simplify();
        public static MathObject atan(MathObject obj) => new Atan(obj).Simplify();
    }
}
