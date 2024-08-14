using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class ShadowMap
    {
        private float[,] depthMap;
        private int width;
        private int height;
        private LightSource currentLight;
        private float nearClipPlane;
        private float farClipPlane;

        public ShadowMap(int width, int height, float nearClipPlane = 0.1f, float farClipPlane = 100f)
        {
            this.width = width;
            this.height = height;
            this.depthMap = new float[width, height];
            this.nearClipPlane = nearClipPlane;
            this.farClipPlane = farClipPlane;
        }

        public void Generate(List<Object3D> objects, LightSource light)
        {
            this.currentLight = light;

            // Clear the depth map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    depthMap[x, y] = float.MaxValue;
                }
            }

            // Project objects onto the depth map from the light's perspective
            foreach (var obj in objects)
            {
                foreach (var triangle in obj.Triangles)
                {
                    var transformedTriangle = triangle.Transform(obj.Position, obj.Rotation);
                    ProjectTriangleOntoDepthMap(transformedTriangle);
                }
            }
        }

        private void ProjectTriangleOntoDepthMap(Triangle3D triangle)
        {
            // Perspective projection for the shadow map
            Vector3D[] projectedVertices = new Vector3D[3];
            for (int i = 0; i < 3; i++)
            {
                Vector3D v = triangle.Vertices[i];
                // Project the vertex onto the light's view
                Vector3D toLight = v - currentLight.Position;
                float distance = toLight.Length();
                float normalizedDepth = (distance - nearClipPlane) / (farClipPlane - nearClipPlane);
                normalizedDepth = Math.Max(0, Math.Min(normalizedDepth, 1)); // Clamping between 0 and 1

                int x = (int)((v.X - currentLight.Position.X) * width / 20.0 + width / 2.0);
                int y = (int)((v.Y - currentLight.Position.Y) * height / 20.0 + height / 2.0);

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    // Use normalized depth
                    depthMap[x, y] = Math.Min(depthMap[x, y], normalizedDepth);
                }
            }
        }

        public float GetShadowFactor(Vector3D point)
        {
            if (currentLight == null)
            {
                return 1f; // Fully lit if no light source is set
            }

            int x = (int)((point.X - currentLight.Position.X) * width / 20.0 + width / 2.0);
            int y = (int)((point.Y - currentLight.Position.Y) * height / 20.0 + height / 2.0);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                Vector3D toLight = point - currentLight.Position;
                float distance = toLight.Length();
                float normalizedDepth = (distance - nearClipPlane) / (farClipPlane - nearClipPlane);
                normalizedDepth = Math.Max(0, Math.Min(normalizedDepth, 1)); // Clamping between 0 and 1

                float storedDepth = depthMap[x, y];

                if (normalizedDepth > storedDepth + 0.1f)
                {
                    return 0.5f; // Soft shadow
                }
            }

            return 1f; // Fully lit
        }
    }
}
