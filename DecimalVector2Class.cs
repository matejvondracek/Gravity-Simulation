using System;
using DecimalMath;
using Microsoft.Xna.Framework;

public class Class1
{
    public struct DecimalVector2
    {
        public decimal X;
        public decimal Y;

        public DecimalVector2(decimal X, decimal Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public DecimalVector2(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        public static bool operator ==(DecimalVector2 dv1, DecimalVector2 dv2)
        {
            return (dv1.X == dv2.X) && (dv1.Y == dv2.Y);
        }

        public static bool operator !=(DecimalVector2 dv1, DecimalVector2 dv2)
        {
            return (dv1.X != dv2.X) || (dv1.Y != dv2.Y);
        }

        public static DecimalVector2 operator +(DecimalVector2 dv1, DecimalVector2 dv2)
        {
            return new DecimalVector2(dv1.X + dv2.X, dv1.Y + dv2.Y);
        }

        public static DecimalVector2 operator -(DecimalVector2 dv1, DecimalVector2 dv2)
        {
            return new DecimalVector2(dv1.X - dv2.X, dv1.Y - dv2.Y);
        }

        public static DecimalVector2 operator *(DecimalVector2 dv, decimal value)
        {
            return new DecimalVector2(dv.X * value, dv.Y * value);
        }

        public bool Equals(DecimalVector2 dv)
        {
            return this == dv;
        }

        public decimal Length()
        {
            return DecimalEx.Sqrt(DecimalEx.Pow(X, 2) + DecimalEx.Pow(Y, 2));
        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public void Normalize()
        {
            decimal length = Length();
            X /= length;
            Y /= length;
        }
    }
}
