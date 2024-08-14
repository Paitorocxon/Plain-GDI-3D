using System;
using System.Drawing;
using Simple3DEngine;

namespace _3DI.engine
{
    public class Material
    {
        public Bitmap Texture { get; set; }
        public Bitmap Normalmap { get; set; }
        private Color[,] TextureData { get; set; }
        private Vector3D[,] NormalData { get; set; }
        public float Roughness { get; set; }

        public Material(Bitmap texture, Bitmap normalmap = null, float roughness = 0.5f)
        {
            Texture = texture;
            Normalmap = normalmap;
            Roughness = roughness;
            LoadTextureData();
            LoadNormalData();
        }

        private void LoadTextureData()
        {
            TextureData = new Color[Texture.Width, Texture.Height];
            for (int x = 0; x < Texture.Width; x++)
            {
                for (int y = 0; y < Texture.Height; y++)
                {
                    TextureData[x, y] = Texture.GetPixel(x, y);
                }
            }
        }

        private void LoadNormalData()
        {
            if (Normalmap == null) return;

            NormalData = new Vector3D[Normalmap.Width, Normalmap.Height];
            for (int x = 0; x < Normalmap.Width; x++)
            {
                for (int y = 0; y < Normalmap.Height; y++)
                {
                    Color color = Normalmap.GetPixel(x, y);
                    // Convert color to normal vector
                    Vector3D normal = new Vector3D(
                        (color.R / 255.0f) * 2.0f - 1.0f,
                        (color.G / 255.0f) * 2.0f - 1.0f,
                        (color.B / 255.0f) * 2.0f - 1.0f
                    );
                    NormalData[x, y] = normal.Normalize();
                }
            }
        }

        public Color GetColorAtUV(float u, float v)
        {
            u = Math.Max(0, Math.Min(1, u));
            v = Math.Max(0, Math.Min(1, v));

            int x = (int)(u * (Texture.Width - 1));
            int y = (int)(v * (Texture.Height - 1));

            return TextureData[x, y];
        }

        public Vector3D GetNormalAtUV(float u, float v)
        {
            if (Normalmap == null) return new Vector3D(0, 0, 1); // Default normal

            u = Math.Max(0, Math.Min(1, u));
            v = Math.Max(0, Math.Min(1, v));

            int x = (int)(u * (Normalmap.Width - 1));
            int y = (int)(v * (Normalmap.Height - 1));

            return NormalData[x, y];
        }
    }
}
