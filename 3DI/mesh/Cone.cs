using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class Cone : Object3D
    {
        public Cone(Vector3D position, float radius, float height, int segments = 20) : base(position)
        {
            CreateConeMesh(radius, height, segments);
        }

        private void CreateConeMesh(float radius, float height, int segments)
        {
            Vector3D topCenter = new Vector3D(0, height / 2, 0);
            Vector3D baseCenter = new Vector3D(0, -height / 2, 0);

            List<Vector3D> baseVertices = new List<Vector3D>();
            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * (float)Math.PI * i / segments;
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                baseVertices.Add(new Vector3D(x, baseCenter.Y, z));
            }

            for (int i = 0; i < segments; i++)
            {
                Vector3D v1 = baseVertices[i];
                Vector3D v2 = baseVertices[(i + 1) % segments];
                Triangles.Add(new Triangle3D(baseCenter, v1, v2));
            }

            for (int i = 0; i < segments; i++)
            {
                Vector3D v1 = baseVertices[i];
                Vector3D v2 = baseVertices[(i + 1) % segments];
                Triangles.Add(new Triangle3D(topCenter, v1, v2));
            }
        }
    }
}
