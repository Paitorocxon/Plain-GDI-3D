using Simple3DEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DI.engine
{
    public class Sun : LightSource
    {
        public float AmbientLight {  get; set; }
        public Color AmbientLightColor {  get; set; }
        public Sun(Vector3D position, Color color, float intensity):base(position, color, intensity, radius:50000)
        {
            Position = position;
            Color = color;
            Intensity = intensity;
            Radius = 50000;
        }
    }
}
