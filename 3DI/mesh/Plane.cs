using _3DI.engine;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simple3DEngine
{
    public class Plane : Object3D
    {
        public Material mat = new Material(new Bitmap(@"C:\users\Fabian\Desktop\texture.png"), new Bitmap(@"C:\users\Fabian\Desktop\texturenm.png"), 1f);
        public Plane(Vector3D position, float width, float height) : base(position)
        {
            CreatePlaneMesh(width, height);
        }

        private void CreatePlaneMesh(float width, float height)
        {
            // Definiere die Ecken der Plane
            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;

            Vector3D v1 = new Vector3D(-halfWidth, 0, -halfHeight);
            Vector3D v2 = new Vector3D(halfWidth, 0, -halfHeight);
            Vector3D v3 = new Vector3D(-halfWidth, 0, halfHeight);
            Vector3D v4 = new Vector3D(halfWidth, 0, halfHeight);

            // Erstelle zwei Dreiecke, die zusammen die Plane bilden
            Triangles.Add(new Triangle3D(v1, v2, v3, mat));
            Triangles.Add(new Triangle3D(v2, v4, v3, mat));
        }
    }
}
