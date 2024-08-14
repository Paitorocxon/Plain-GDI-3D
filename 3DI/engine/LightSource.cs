using Simple3DEngine;
using System.Drawing;

public class LightSource
{
    public Vector3D Position { set; get; }
    public Color Color { set; get; }
    public float Intensity { set; get; }
    public float Radius { set; get; } // Neuer Parameter für den Leuchtradius

    public LightSource(Vector3D position, Color color, float intensity, float radius = 10.0f)
    {
        Position = position;
        Color = color;
        Intensity = intensity;
        Radius = radius;
    }
}
