using Simple3DEngine;
using System.Drawing;

public class Triangle2D
{
    public Point[] Vertices { get; private set; }
    public Vector3D Normal { get; private set; }

    public Triangle2D(Point v1, Point v2, Point v3, Vector3D normal)
    {
        Vertices = new Point[] { v1, v2, v3 };
        Normal = normal;
    }
}