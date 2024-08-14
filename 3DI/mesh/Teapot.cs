using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class Teapot : Object3D
    {
        public Teapot(Vector3D position, float size) : base(position)
        {
            CreateTeapotMesh(size);
        }

        private void CreateTeapotMesh(float size)
        {
            float bodyRadius = size * 0.5f;
            float bodyHeight = size * 0.4f;
            float spoutLength = size * 0.4f;
            float spoutRadius = size * 0.1f;
            float handleRadius = size * 0.2f;
            float lidRadius = size * 0.3f;
            float lidHeight = size * 0.1f;

            CreateSpheroidMesh(bodyRadius, bodyHeight, 16, 8);
            CreateConeMesh(new Vector3D(bodyRadius + spoutLength / 2, 0, 0), spoutRadius, spoutLength, 8);
            CreateTorusMesh(new Vector3D(-bodyRadius, 0, 0), handleRadius, spoutRadius, 12, 8);
            CreateConeMesh(new Vector3D(0, bodyHeight / 2 + lidHeight / 2, 0), lidRadius, lidHeight, 16);
        }

        private void CreateSpheroidMesh(float radiusXZ, float radiusY, int segmentsXZ, int segmentsY)
        {
            for (int y = 0; y < segmentsY; y++)
            {
                float angleY1 = (float)Math.PI * y / segmentsY;
                float angleY2 = (float)Math.PI * (y + 1) / segmentsY;

                for (int xz = 0; xz < segmentsXZ; xz++)
                {
                    float angleXZ1 = 2 * (float)Math.PI * xz / segmentsXZ;
                    float angleXZ2 = 2 * (float)Math.PI * (xz + 1) / segmentsXZ;

                    Vector3D v1 = new Vector3D(
                        radiusXZ * (float)Math.Sin(angleY1) * (float)Math.Cos(angleXZ1),
                        radiusY * (float)Math.Cos(angleY1),
                        radiusXZ * (float)Math.Sin(angleY1) * (float)Math.Sin(angleXZ1));

                    Vector3D v2 = new Vector3D(
                        radiusXZ * (float)Math.Sin(angleY1) * (float)Math.Cos(angleXZ2),
                        radiusY * (float)Math.Cos(angleY1),
                        radiusXZ * (float)Math.Sin(angleY1) * (float)Math.Sin(angleXZ2));

                    Vector3D v3 = new Vector3D(
                        radiusXZ * (float)Math.Sin(angleY2) * (float)Math.Cos(angleXZ1),
                        radiusY * (float)Math.Cos(angleY2),
                        radiusXZ * (float)Math.Sin(angleY2) * (float)Math.Sin(angleXZ1));

                    Vector3D v4 = new Vector3D(
                        radiusXZ * (float)Math.Sin(angleY2) * (float)Math.Cos(angleXZ2),
                        radiusY * (float)Math.Cos(angleY2),
                        radiusXZ * (float)Math.Sin(angleY2) * (float)Math.Sin(angleXZ2));

                    Triangles.Add(new Triangle3D(v1, v2, v3));
                    Triangles.Add(new Triangle3D(v2, v4, v3));
                }
            }
        }

        private void CreateConeMesh(Vector3D center, float radius, float height, int segments)
        {
            Vector3D topCenter = new Vector3D(center.X, center.Y + height / 2, center.Z);
            Vector3D baseCenter = new Vector3D(center.X, center.Y - height / 2, center.Z);

            List<Vector3D> baseVertices = new List<Vector3D>();
            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * (float)Math.PI * i / segments;
                float x = radius * (float)Math.Cos(angle);
                float z = radius * (float)Math.Sin(angle);
                baseVertices.Add(new Vector3D(baseCenter.X + x, baseCenter.Y, baseCenter.Z + z));
            }

            for (int i = 0; i < segments; i++)
            {
                Vector3D v1 = baseVertices[i];
                Vector3D v2 = baseVertices[(i + 1) % segments];
                Triangles.Add(new Triangle3D(baseCenter, v1, v2));
                Triangles.Add(new Triangle3D(topCenter, v1, v2));
            }
        }

        private void CreateTorusMesh(Vector3D center, float majorRadius, float minorRadius, int majorSegments, int minorSegments)
        {
            for (int i = 0; i < majorSegments; i++)
            {
                float u = 2 * (float)Math.PI * i / majorSegments;
                float u1 = 2 * (float)Math.PI * (i + 1) / majorSegments;

                for (int j = 0; j < minorSegments; j++)
                {
                    float v = 2 * (float)Math.PI * j / minorSegments;
                    float v1 = 2 * (float)Math.PI * (j + 1) / minorSegments;

                    Vector3D p1 = CalculateTorusPoint(center, majorRadius, minorRadius, u, v);
                    Vector3D p2 = CalculateTorusPoint(center, majorRadius, minorRadius, u1, v);
                    Vector3D p3 = CalculateTorusPoint(center, majorRadius, minorRadius, u, v1);
                    Vector3D p4 = CalculateTorusPoint(center, majorRadius, minorRadius, u1, v1);

                    Triangles.Add(new Triangle3D(p1, p2, p3));
                    Triangles.Add(new Triangle3D(p2, p4, p3));
                }
            }
        }

        private Vector3D CalculateTorusPoint(Vector3D center, float majorRadius, float minorRadius, float u, float v)
        {
            float x = (majorRadius + minorRadius * (float)Math.Cos(v)) * (float)Math.Cos(u);
            float y = minorRadius * (float)Math.Sin(v);
            float z = (majorRadius + minorRadius * (float)Math.Cos(v)) * (float)Math.Sin(u);
            return new Vector3D(center.X + x, center.Y + y, center.Z + z);
        }
    }
}
