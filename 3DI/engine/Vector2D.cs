using System;
using System.Drawing; // System.Drawing benötigt für PointF

namespace _3DI.engine
{
    public class Vector2D
    {
        public float X, Y;

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2D Lerp(Vector2D start, Vector2D end, float t)
        {
            return new Vector2D(
                start.X + (end.X - start.X) * t,
                start.Y + (end.Y - start.Y) * t
            );
        }

        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }

        public override string ToString()
        {
            return $"V2(X:{X} Y:{Y})";
        }
    }
}
