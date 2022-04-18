using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOperation
{
    public class Equation : MathObject
    {
        public enum Operators { Equal, NotEqual, LessThan, GreaterThan }

        public readonly MathObject a;
        public readonly MathObject b;

        public Operators Operator;

        public Equation(MathObject x, MathObject y)
        { a = x; b = y; Operator = Operators.Equal; }

        public Equation(MathObject x, MathObject y, Operators op)
        { a = x; b = y; Operator = op; }

        public override string FullForm()
        {
            if (Operator == Operators.Equal) return a + " == " + b;
            if (Operator == Operators.NotEqual) return a + " != " + b;
            if (Operator == Operators.LessThan) return a + " < " + b;
            if (Operator == Operators.GreaterThan) return a + " > " + b;
            throw new Exception();
        }

        public override bool Equals(object obj) =>
            obj is Equation &&
            a.Equals((obj as Equation).a) &&
            b.Equals((obj as Equation).b) &&
            Operator == (obj as Equation).Operator;

        Boolean ToBoolean()
        {
            if (a is Bool && b is Bool) return (a as Bool).Equals(b);

            if (a is Equation && b is Equation) return (a as Equation).Equals(b);

            if (a is Integer && b is Integer) return ((Integer)a).Equals(b);
            if (a is DoubleFloat && b is DoubleFloat) return ((DoubleFloat)a).Equals(b);
            if (a is Symbol && b is Symbol) return ((Symbol)a).Equals(b);
            if (a is Sum && b is Sum) return ((Sum)a).Equals(b);
            if (a is Product && b is Product) return ((Product)a).Equals(b);
            if (a is Fraction && b is Fraction) return ((Fraction)a).Equals(b);
            if (a is Power && b is Power) return ((Power)a).Equals(b);
            if (a is Function && b is Function) return ((Function)a).Equals(b);

            if ((((object)a) == null) && (((object)b) == null)) return true;

            if (((object)a) == null) return false;

            if (((object)b) == null) return false;

            if (a.GetType() != b.GetType()) return false;

            Console.WriteLine("" + a.GetType() + " " + b.GetType());

            throw new Exception();
        }

        public static implicit operator Boolean(Equation eq)
        {
            if (eq.Operator == Operators.Equal)
                return (eq.a == eq.b).ToBoolean();

            if (eq.Operator == Operators.NotEqual)
                return !((eq.a == eq.b).ToBoolean());

            if (eq.Operator == Operators.LessThan)
                if (eq.a is Number && eq.b is Number)
                    return (eq.a as Number).ToDouble().val < (eq.b as Number).ToDouble().val;

            if (eq.Operator == Operators.GreaterThan)
                if (eq.a is Number && eq.b is Number)
                    return (eq.a as Number).ToDouble().val > (eq.b as Number).ToDouble().val;

            throw new Exception();
        }

        public MathObject Simplify()
        {
            if (a is Number && b is Number) return (bool)this;

            return this;
        }

        public override int GetHashCode() => new { a, b }.GetHashCode();

    }
}
