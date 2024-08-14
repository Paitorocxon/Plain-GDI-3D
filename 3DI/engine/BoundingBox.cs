using System;

namespace Simple3DEngine
{
    public class BoundingBox
    {
        public Vector3D Min { get; set; }
        public Vector3D Max { get; set; }

        public BoundingBox(Vector3D min, Vector3D max)
        {
            Min = min;
            Max = max;
        }

        public bool Intersects(BoundingBox other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }
}
