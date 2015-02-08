using Saga.Enumarations;
using System;

namespace Saga.Structures
{
    [Serializable()]
    public struct AdditionInteger
    {
        private int _currentvalue;
        private int _cumv;

        [System.Diagnostics.DebuggerHidden()]
        public static implicit operator AdditionInteger(int x)
        {
            AdditionInteger b = new AdditionInteger();

            if (x >= 20000)
            {
                int mult = x - 20000;
                b._cumv += mult;
            }
            else if (x <= -20000)
            {
                int mult = x + 20000;
                b._cumv += mult;
            }
            else
            {
                b._currentvalue += x;
            }

            return b;
        }

        [System.Diagnostics.DebuggerHidden()]
        public static explicit operator int(AdditionInteger x)
        {
            return x._currentvalue;
        }

        [System.Diagnostics.DebuggerHidden()]
        public AdditionInteger(int value)
        {
            _cumv = 0;
            _currentvalue = value;
        }

        [System.Diagnostics.DebuggerHidden()]
        public AdditionInteger(int cumv, int value)
        {
            _cumv = cumv;
            _currentvalue = value;
        }

        // Declare which operator to overload (+),
        // the types that can be added (two Complex objects),
        // and the return type (Complex):
        [System.Diagnostics.DebuggerHidden()]
        public static AdditionInteger operator +(AdditionInteger c1, AdditionInteger c2)
        {
            decimal c1Scalar = 1 + (((decimal)c1._cumv) / 1000);
            decimal c2Scalar = 1 + (((decimal)c1._cumv + c2._cumv) / 1000);
            int newValue = (int)(((decimal)c1._currentvalue / c1Scalar) * (c2Scalar)) + c2._currentvalue;
            int newMult = c1._cumv + c2._cumv;
            return new AdditionInteger(newMult, newValue);
        }

        // Declare which operator to overload (+),
        // the types that can be added (two Complex objects),
        // and the return type (Complex):
        [System.Diagnostics.DebuggerHidden()]
        public static AdditionInteger operator -(AdditionInteger c1, AdditionInteger c2)
        {
            decimal c1Scalar = 1 + (((decimal)c1._cumv) / 1000);
            decimal c2Scalar = 1 + (((decimal)c1._cumv - c2._cumv) / 1000);
            int newValue = (int)(((decimal)c1._currentvalue / c1Scalar) * (c2Scalar)) - c2._currentvalue;
            int newMult = c1._cumv - c2._cumv;
            return new AdditionInteger(newMult, newValue);
        }

        // Override the ToString() method to display a complex number in the traditional format:
        public override string ToString()
        {
            return _currentvalue.ToString();
        }
    }

    public struct AdditionValue
    {
        public uint additionid;
        public int value;
        public object sender;
        public object target;
        public AdditionContext context;
    }
}