using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class Tree : Object3D
    {
        private float trunkRadius;
        private float trunkHeight;
        private float canopyRadius;
        private int trunkSegments;
        private int canopySegments;

        public Tree(Vector3D position, float trunkRadius, float trunkHeight, float canopyRadius, int trunkSegments = 8, int canopySegments = 12)
            : base(position)
        {
            this.trunkRadius = trunkRadius;
            this.trunkHeight = trunkHeight;
            this.canopyRadius = canopyRadius;
            this.trunkSegments = trunkSegments;
            this.canopySegments = canopySegments;

            CreateTrunk();
            CreateCanopy();
        }

        private void CreateTrunk()
        {
            for (int i = 0; i < trunkSegments; i++)
            {
                float angle1 = i * 2 * (float)Math.PI / trunkSegments;
                float angle2 = (i + 1) * 2 * (float)Math.PI / trunkSegments;

                Vector3D bottom1 = new Vector3D(trunkRadius * (float)Math.Cos(angle1), 0, trunkRadius * (float)Math.Sin(angle1));
                Vector3D bottom2 = new Vector3D(trunkRadius * (float)Math.Cos(angle2), 0, trunkRadius * (float)Math.Sin(angle2));
                Vector3D top1 = new Vector3D(trunkRadius * (float)Math.Cos(angle1), trunkHeight, trunkRadius * (float)Math.Sin(angle1));
                Vector3D top2 = new Vector3D(trunkRadius * (float)Math.Cos(angle2), trunkHeight, trunkRadius * (float)Math.Sin(angle2));

                Triangles.Add(new Triangle3D(bottom1, bottom2, new Vector3D(0, 0, 0)));
                Triangles.Add(new Triangle3D(top1, top2, new Vector3D(0, trunkHeight, 0)));
                Triangles.Add(new Triangle3D(bottom1, top1, top2));
                Triangles.Add(new Triangle3D(bottom1, top2, bottom2));
            }
        }

        private void CreateCanopy()
        {
            for (int i = 0; i < canopySegments; i++)
            {
                for (int j = 0; j < canopySegments; j++)
                {
                    float theta1 = i * (float)Math.PI * 2 / canopySegments;
                    float theta2 = (i + 1) * (float)Math.PI * 2 / canopySegments;
                    float phi1 = j * (float)Math.PI / canopySegments;
                    float phi2 = (j + 1) * (float)Math.PI / canopySegments;

                    Vector3D p1 = new Vector3D(
                        canopyRadius * (float)Math.Sin(phi1) * (float)Math.Cos(theta1),
                        canopyRadius * (float)Math.Cos(phi1),
                        canopyRadius * (float)Math.Sin(phi1) * (float)Math.Sin(theta1));

                    Vector3D p2 = new Vector3D(
                        canopyRadius * (float)Math.Sin(phi1) * (float)Math.Cos(theta2),
                        canopyRadius * (float)Math.Cos(phi1),
                        canopyRadius * (float)Math.Sin(phi1) * (float)Math.Sin(theta2));

                    Vector3D p3 = new Vector3D(
                        canopyRadius * (float)Math.Sin(phi2) * (float)Math.Cos(theta2),
                        canopyRadius * (float)Math.Cos(phi2),
                        canopyRadius * (float)Math.Sin(phi2) * (float)Math.Sin(theta2));

                    Vector3D p4 = new Vector3D(
                        canopyRadius * (float)Math.Sin(phi2) * (float)Math.Cos(theta1),
                        canopyRadius * (float)Math.Cos(phi2),
                        canopyRadius * (float)Math.Sin(phi2) * (float)Math.Sin(theta1));

                    Triangles.Add(new Triangle3D(p1, p2, p3));
                    Triangles.Add(new Triangle3D(p1, p3, p4));
                }
            }
        }
    }
}
