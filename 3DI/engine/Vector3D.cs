using System;

namespace Simple3DEngine
{
    public class Vector3D
    {
        public float X, Y, Z;
        public static readonly Vector3D Zero = new Vector3D(0.0000001f, 0.0000001f, 0.0000001f);

        public Vector3D(float x, float y, float z)
        {
            X = x + 0.0000001f;
            Y = y + 0.0000001f;
            Z = z + 0.0000001f;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3D operator -(Vector3D a)
        {
            return new Vector3D(-a.X, -a.Y, -a.Z);
        }
        public static Vector3D operator *(Vector3D a, float scalar)
        {
            return new Vector3D(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        public static Vector3D operator /(Vector3D a, float scalar)
        {
            return new Vector3D(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }

        public static Vector3D FromEulerAngles(float yawDegrees, float pitchDegrees = 0, float rollDegrees = 0)
        {
            float yawRadians = yawDegrees * (float)Math.PI / 180f;
            float pitchRadians = pitchDegrees * (float)Math.PI / 180f;

            float cosYaw = (float)Math.Cos(yawRadians);
            float sinYaw = (float)Math.Sin(yawRadians);
            float cosPitch = (float)Math.Cos(pitchRadians);
            float sinPitch = (float)Math.Sin(pitchRadians);

            return new Vector3D(
                sinYaw * cosPitch,
                -sinPitch,
                cosYaw * cosPitch
            ).Normalize();
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }
        public static Vector3D CrossProduct(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }


        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3D Normalize()
        {
            float length = Length();
            return new Vector3D(X / length, Y / length, Z / length);
        }
        public static float Distance(Vector3D a, Vector3D b)
        {
            // Berechnet die Distanz zwischen zwei Punkten im 3D-Raum
            return (a - b).Length();
        }

        public static Vector3D Lerp(Vector3D start, Vector3D end, float t)
        {
            return new Vector3D(
                start.X + (end.X - start.X) * t,
                start.Y + (end.Y - start.Y) * t,
                start.Z + (end.Z - start.Z) * t
            );
        }
        public override string ToString()
        {
            return ($"V3(X:{X} Y:{Y} Z:{Z})");
        }
    }
}
