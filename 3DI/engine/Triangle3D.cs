using _3DI.engine;
using System.Drawing;
using System.Linq;

namespace Simple3DEngine
{
    public class Triangle3D
    {
        public Vector3D[] Vertices;
        public Vector3D Normal;
        public Vector2D[] TextureCoordinates { get; set; }
        public Material Material { get; set; }


        public Color Color { get; private set; } // Die Farbe des Dreiecks
        public bool Materialized = false;

        public Triangle3D(Vector3D v1, Vector3D v2, Vector3D v3, Color? color = null)
        {
            Color = color ?? Color.Red; // Setze Standardwert auf Weiß
            Vertices = new Vector3D[] { v1, v2, v3 };
            CalculateNormal();
            EnsureClockwiseOrder();

            TextureCoordinates = new Vector2D[] // Proforma, weil darf nich null sein
            {
                new Vector2D(0, 0),
                new Vector2D(1, 0),
                new Vector2D(0, 1)
            };
        }

        public Triangle3D(Vector3D v1, Vector3D v2, Vector3D v3, Material material)
        {
            this.Materialized = true;
            this.Material = material; //Setze das Material
            Vertices = new Vector3D[] { v1, v2, v3 };
            TextureCoordinates = new Vector2D[]
            {
                    new Vector2D(0, 0),
                    new Vector2D(1, 0),
                    new Vector2D(0, 1)
            };
            CalculateNormal();
            EnsureClockwiseOrder();

            if (material?.Texture != null) AutoUVMap();

        }
        private void CalculateTextureCoordinates()
        {
            Vector3D edge1 = Vertices[1] - Vertices[0];
            Vector3D edge2 = Vertices[2] - Vertices[0];
            TextureCoordinates = new Vector2D[]
            {
                new Vector2D(0, 0),
                new Vector2D(edge1.Length() / Material.Texture.Width, 0),
                new Vector2D(0, edge2.Length() / Material.Texture.Height)
            };
        }
        private void AutoUVMap()
        {
            Vector3D normal = Normal.Normalize();
            Vector3D tangent = Vector3D.Cross(normal, new Vector3D(0, 1, 0)).Normalize();
            Vector3D bitangent = Vector3D.Cross(normal, tangent);

            TextureCoordinates = new Vector2D[3];
            for (int i = 0; i < 3; i++)
            {
                float u = Vector3D.Dot(Vertices[i], tangent);
                float v = Vector3D.Dot(Vertices[i], bitangent);
                TextureCoordinates[i] = new Vector2D(u, v);
            }

            // Normalisieren der UV-Koordinaten
            float minU = TextureCoordinates.Min(uv => uv.X);
            float minV = TextureCoordinates.Min(uv => uv.Y);
            float maxU = TextureCoordinates.Max(uv => uv.X);
            float maxV = TextureCoordinates.Max(uv => uv.Y);

            for (int i = 0; i < 3; i++)
            {
                TextureCoordinates[i].X = (TextureCoordinates[i].X - minU) / (maxU - minU);
                TextureCoordinates[i].Y = (TextureCoordinates[i].Y - minV) / (maxV - minV);
            }
        }
        public void EnsureClockwiseOrder()
        {
            Vector3D edge1 = Vertices[1] - Vertices[0];
            Vector3D edge2 = Vertices[2] - Vertices[0];
            Vector3D normal = Vector3D.Cross(edge1, edge2);

            if (Vector3D.Dot(normal, Normal) < 0)
            {
                // Vertausche zwei Vertices, um die Reihenfolge umzukehren
                Vector3D temp = Vertices[1];
                Vertices[1] = Vertices[2];
                Vertices[2] = temp;
                CalculateNormal(); // Normale neu berechnen
            }
        }

        private void CalculateNormal()
        {
            Vector3D edge1 = Vertices[1] - Vertices[0];
            Vector3D edge2 = Vertices[2] - Vertices[0];
            Normal = Vector3D.Cross(edge2, edge1).Normalize();
        }

        public Triangle3D Transform(Vector3D position, Vector3D rotation)
        {
            Vector3D[] transformedVertices = new Vector3D[3];
            for (int i = 0; i < 3; i++)
            {
                Vector3D v = RotateVector(Vertices[i], rotation);
                transformedVertices[i] = v + position;
            }

            Triangle3D transformedTriangle;
            if (Materialized)
            {
                transformedTriangle = new Triangle3D(transformedVertices[0], transformedVertices[1], transformedVertices[2], this.Material);
            }
            else
            {
                transformedTriangle = new Triangle3D(transformedVertices[0], transformedVertices[1], transformedVertices[2], this.Color);
            }

            transformedTriangle.Normal = RotateVector(Normal, rotation).Normalize();
            transformedTriangle.TextureCoordinates = this.TextureCoordinates; // Übertragen Sie die Texturkoordinaten

            return transformedTriangle;
        }

        private Vector3D RotateVector(Vector3D v, Vector3D rotation)
        {
            float cosX = (float)System.Math.Cos(rotation.X);
            float sinX = (float)System.Math.Sin(rotation.X);
            float cosY = (float)System.Math.Cos(rotation.Y);
            float sinY = (float)System.Math.Sin(rotation.Y);
            float cosZ = (float)System.Math.Cos(rotation.Z);
            float sinZ = (float)System.Math.Sin(rotation.Z);

            // Rotation um X-Achse
            float y1 = v.Y * cosX - v.Z * sinX;
            float z1 = v.Y * sinX + v.Z * cosX;

            // Rotation um Y-Achse
            float x2 = v.X * cosY + z1 * sinY;
            float z2 = -v.X * sinY + z1 * cosY;

            // Rotation um Z-Achse
            float x3 = x2 * cosZ - y1 * sinZ;
            float y3 = x2 * sinZ + y1 * cosZ;

            return new Vector3D(x3, y3, z2);
        }

    }
}