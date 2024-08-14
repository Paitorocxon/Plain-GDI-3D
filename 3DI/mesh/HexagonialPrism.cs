using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class HexagonalPrism : Object3D
    {
        public HexagonalPrism(Vector3D position, float radius, float height) : base(position)
        {
            CreateHexagonalPrismMesh(radius, height);
        }

        private void CreateHexagonalPrismMesh(float radius, float height)
        {
            // Create vertices for the base hexagon
            List<Vector3D> baseVertices = new List<Vector3D>();
            List<Vector3D> topVertices = new List<Vector3D>();
            int segments = 6;

            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * (float)Math.PI * i / segments;
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                baseVertices.Add(new Vector3D(x, 0, z));
                topVertices.Add(new Vector3D(x, height, z));
            }

            // Create base face
            for (int i = 1; i < segments - 1; i++)
            {
                Triangles.Add(new Triangle3D(
                    baseVertices[0],
                    baseVertices[i],
                    baseVertices[i + 1]
                ));
            }

            // Create top face
            for (int i = 1; i < segments - 1; i++)
            {
                Triangles.Add(new Triangle3D(
                    topVertices[0],
                    topVertices[i],
                    topVertices[i + 1]
                ));
            }

            // Create side faces
            for (int i = 0; i < segments; i++)
            {
                Vector3D v1 = baseVertices[i];
                Vector3D v2 = baseVertices[(i + 1) % segments];
                Vector3D v3 = topVertices[i];
                Vector3D v4 = topVertices[(i + 1) % segments];

                Triangles.Add(new Triangle3D(v1, v2, v3));
                Triangles.Add(new Triangle3D(v2, v4, v3));
            }
        }
    }

}
