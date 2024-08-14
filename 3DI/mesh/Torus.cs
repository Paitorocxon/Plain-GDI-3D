using _3DI.engine;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simple3DEngine
{
    public class Torus : Object3D
    {
        private float innerRadius;
        private float outerRadius;
        private int radialSegments;
        private int tubularSegments;
        public Material Material = new Material(new Bitmap(@"C:\users\Fabian\Desktop\texture.png"), new Bitmap(@"C:\users\Fabian\Desktop\texturenm.png"), 1f);
        public Torus(Vector3D position, float innerRadius, float outerRadius, int radialSegments, int tubularSegments)
            : base(position)
        {
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.radialSegments = radialSegments;
            this.tubularSegments = tubularSegments;

            GenerateTorus();
        }

        private void GenerateTorus()
        {
            Triangles = new List<Triangle3D>();

            // Generate vertices for the torus
            Vector3D[,] vertices = new Vector3D[radialSegments, tubularSegments];

            for (int i = 0; i < radialSegments; i++)
            {
                float theta = i * 2 * (float)Math.PI / radialSegments;
                float cosTheta = (float)Math.Cos(theta);
                float sinTheta = (float)Math.Sin(theta);

                for (int j = 0; j < tubularSegments; j++)
                {
                    float phi = j * 2 * (float)Math.PI / tubularSegments;
                    float cosPhi = (float)Math.Cos(phi);
                    float sinPhi = (float)Math.Sin(phi);

                    float x = (outerRadius + innerRadius * cosPhi) * cosTheta;
                    float y = (outerRadius + innerRadius * cosPhi) * sinTheta;
                    float z = innerRadius * sinPhi;

                    vertices[i, j] = new Vector3D(x, y, z);
                }
            }

            // Generate faces (triangles) for the torus
            for (int i = 0; i < radialSegments; i++)
            {
                for (int j = 0; j < tubularSegments; j++)
                {
                    int i0 = i;
                    int i1 = (i + 1) % radialSegments;
                    int j0 = j;
                    int j1 = (j + 1) % tubularSegments;

                    Vector3D v0 = vertices[i0, j0];
                    Vector3D v1 = vertices[i1, j0];
                    Vector3D v2 = vertices[i1, j1];
                    Vector3D v3 = vertices[i0, j1];

                    //Triangles.Add(new Triangle3D(v0, v1, v2, System.Drawing.Color.Red));
                    //Triangles.Add(new Triangle3D(v0, v2, v3, System.Drawing.Color.Red));
                    Triangles.Add(new Triangle3D(v0, v1, v2, Color.Red));
                    Triangles.Add(new Triangle3D(v0, v2, v3, Color.Red));
                }
            }
        }

        public new Torus Transform(Vector3D position, Vector3D rotation)
        {
            Torus transformedTorus = new Torus(position, innerRadius, outerRadius, radialSegments, tubularSegments)
            {
                Rotation = rotation,
                EnableCulling = this.EnableCulling
            };

            List<Triangle3D> transformedTriangles = new List<Triangle3D>();

            foreach (Triangle3D triangle in Triangles)
            {
                transformedTriangles.Add(triangle.Transform(position, rotation));
            }

            transformedTorus.Triangles = transformedTriangles;
            return transformedTorus;
        }
    }
}
