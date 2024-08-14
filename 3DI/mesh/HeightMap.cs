using _3DI.engine;
using System;
using System.Drawing;

namespace Simple3DEngine
{
    public class HeightMap : Object3D
    {
        private int width;
        private int height;
        private int intensity;
        private float[,] heightData;
        private float scale;
        public Material mat = new Material(new Bitmap(@"C:\users\Fabian\Desktop\texture.png"), new Bitmap(@"C:\users\Fabian\Desktop\texturenm.png"), 1f);

        public HeightMap(Vector3D position, int width, int height, int intensity, float scale = 1.0f)
            : base(position)
        {
            this.width = width;
            this.height = height;
            this.scale = scale;
            this.intensity = intensity;
            this.heightData = new float[width, height];

            GenerateRandomHeightData();
            CreateMesh();
        }

        private void GenerateRandomHeightData()
        {
            Random random = new Random();
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    heightData[x, z] = (float)random.NextDouble() * intensity;
                }
            }
            SmoothHeightData();
        }

        private void SmoothHeightData()
        {
            for (int iterations = 0; iterations < 3; iterations++)
            {
                float[,] smoothedData = new float[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        float sum = 0;
                        int count = 0;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dz = -1; dz <= 1; dz++)
                            {
                                int nx = x + dx;
                                int nz = z + dz;
                                if (nx >= 0 && nx < width && nz >= 0 && nz < height)
                                {
                                    sum += heightData[nx, nz];
                                    count++;
                                }
                            }
                        }
                        smoothedData[x, z] = sum / count;
                    }
                }
                heightData = smoothedData;
            }
        }

        private void CreateMesh()
        {
            Triangles.Clear(); // Clear existing triangles if any
            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < height - 1; z++)
                {
                    Vector3D v1 = new Vector3D(x * scale, heightData[x, z] * scale, z * scale);
                    Vector3D v2 = new Vector3D((x + 1) * scale, heightData[x + 1, z] * scale, z * scale);
                    Vector3D v3 = new Vector3D(x * scale, heightData[x, z + 1] * scale, (z + 1) * scale);
                    Vector3D v4 = new Vector3D((x + 1) * scale, heightData[x + 1, z + 1] * scale, (z + 1) * scale);

                    // Note the change here: the order of the vertices is reversed
                    Triangles.Add(new Triangle3D(v1, v3, v2, System.Drawing.Color.Green));
                    Triangles.Add(new Triangle3D(v2, v3, v4, System.Drawing.Color.GreenYellow));
                }
            }
        }

        public void SetHeightData(float[,] newHeightData)
        {
            if (newHeightData.GetLength(0) != width || newHeightData.GetLength(1) != height)
            {
                throw new ArgumentException("New height data dimensions must match the current dimensions.");
            }

            heightData = newHeightData;
            Triangles.Clear();
            CreateMesh();
        }
    }
}
