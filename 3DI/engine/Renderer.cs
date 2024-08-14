
using _3DI.engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Numerics;
using System.Runtime.ConstrainedExecution;


namespace Simple3DEngine
{
    public class Renderer
    {

        private int width;
        private int height;

        public bool GridMesh = false;   // DEBUG PROPERTY
        public Sun Sun = new Sun(new Vector3D(6000, 3000, 6000), Color.Wheat, 1.2f);
        private Graphics graphics;
        private Camera camera;
        public List<LightSource> lightSources;
        public List<AudioSource3D> audioSources;
        private ShadowMap shadowMap;
        private Image lightBulbImage;
        private Image sunImage;
        private Image skyboxTexture;
        public float CullingTolerance = 0.01f; // Kulanz für Backface-Culling
        private PhysicsEngine physicsEngine;
        private Bitmap renderBuffer;

        Graphics renderGraphics;
        public Renderer(Graphics graphics, Camera camera, List<LightSource> lightSources, string lightBulbImagePath)
        {
            //width = (int)graphics.VisibleClipBounds.Width;
            //height = (int)graphics.VisibleClipBounds.Height;
            width = (int)camera.GetResolution().X;
            height = (int)camera.GetResolution().Y;
            this.renderBuffer = new Bitmap(width, height);

            this.graphics = graphics;
            this.camera = camera;
            this.lightSources = lightSources;
            this.lightSources.Add(Sun);
            this.audioSources = audioSources;
            this.shadowMap = new ShadowMap(width, height);
            this.lightBulbImage = Image.FromFile(lightBulbImagePath);
            this.sunImage = Image.FromFile("sun.png");
            this.skyboxTexture = Image.FromFile("skybox.png");
            this.physicsEngine = new PhysicsEngine();
            renderGraphics = Graphics.FromImage(renderBuffer);
        }
        int tick = 0;
        public void Render(List<Object3D> objects)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();
            // Hintergrund (Skybox) zeichnen
            //graphics.Clear(Color.DarkBlue);
            graphics.DrawImage(renderBuffer, 0, 0, width,height);
            ClearRenderBuffer();
            DrawSkybox();
            tick++;
            //if (tick > 2) {
                tick = 0;
                physicsEngine.ProcessCollisions(objects);
            //}



            shadowMap.Generate(objects, Sun);
            
            var trianglesToRender = new List<(Triangle3D, Triangle2D)>();

            // Transformiere und projiziere alle Dreiecke, sortiere nach Tiefe
            foreach (var obj in objects)
            {
                foreach (var triangle in obj.Triangles)
                {
                    var transformedTriangle = triangle.Transform(obj.Position, obj.Rotation);
                    var projectedTriangle = camera.ProjectTriangle(transformedTriangle, width, height);

                    if (projectedTriangle != null)
                    {
                        if (!obj.EnableCulling || !IsBackface(transformedTriangle, camera.Position))
                        {
                            trianglesToRender.Add((transformedTriangle, projectedTriangle));
                        }
                    }
                }
            }

            // Sortiere Dreiecke nach der durchschnittlichen Z-Koordinate der Punkte für die Tiefe
            //trianglesToRender.Sort((a, b) =>
            //{
            //    float avgZA = (a.Item1.Vertices[0].Z + a.Item1.Vertices[1].Z + a.Item1.Vertices[2].Z) / 3;
            //    float avgZB = (b.Item1.Vertices[0].Z + b.Item1.Vertices[1].Z + b.Item1.Vertices[2].Z) / 3;
            //    return avgZB.CompareTo(avgZA); // Vergleich nach absteigender Tiefe
            //});

            trianglesToRender.Sort((a, b) =>
            {
                Vector3D centerA = (a.Item1.Vertices[0] + a.Item1.Vertices[1] + a.Item1.Vertices[2]) * (1.0f / 3.0f);
                Vector3D centerB = (b.Item1.Vertices[0] + b.Item1.Vertices[1] + b.Item1.Vertices[2]) * (1.0f / 3.0f);

                float distanceA = (centerA - camera.Position).Length();
                float distanceB = (centerB - camera.Position).Length();

                return distanceB.CompareTo(distanceA); // Sort from farthest to nearest
            });


            //
            //trianglesToRender.Sort((a, b) => {
            //    float avgZA = (a.Item1.Vertices[0].Length() + a.Item1.Vertices[1].Length() + a.Item1.Vertices[2].Length()) / 3f;
            //    float avgZB = (b.Item1.Vertices[0].Length() + b.Item1.Vertices[1].Length() + b.Item1.Vertices[2].Length()) / 3f;
            //    return avgZB.CompareTo(avgZA); // Sort descending (farthest to nearest)
            //});
            DrawSun();
            // Zeichne die sortierten Dreiecke
            foreach (var (transformedTriangle, projectedTriangle) in trianglesToRender)
            {
                DrawTriangle(projectedTriangle, transformedTriangle);
            }
            //Console.WriteLine(verticies);

            // Zeichne Lichtquellen
            DrawLightSources();

            stopwatch.Stop();

            physicsEngine.ProcessCollisions(objects);
            graphics.DrawString(stopwatch.ElapsedMilliseconds.ToString() + "/" + 1000 + " @" + 1000/stopwatch.ElapsedMilliseconds + "FPS", new Font("Arial", 12), Brushes.Wheat, new Point(0,0));
            graphics.DrawString(trianglesToRender.Count.ToString() + " Verticies Drawn", new Font("Arial", 12), Brushes.Wheat,new Point(0,13));
            
        }
        private void ClearRenderBuffer()
        {
            renderGraphics.Clear(Color.Transparent);
        }
        private bool IsBackface(Triangle3D triangle, Vector3D cameraPosition)
        {
            Vector3D toCamera = (cameraPosition - triangle.Vertices[0]).Normalize();
            float dotProduct = Vector3D.Dot(triangle.Normal, toCamera);
            return dotProduct <= CullingTolerance;
        }
        public void ToHighQuality()
        {
            this.graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            this.graphics.CompositingQuality = CompositingQuality.HighQuality;
            this.graphics.SmoothingMode = SmoothingMode.HighQuality;
            this.graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            this.graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public void ToLowQuality()
        {
            this.graphics.InterpolationMode = InterpolationMode.Low;
            this.graphics.CompositingQuality = CompositingQuality.HighSpeed;
            this.graphics.SmoothingMode = SmoothingMode.HighSpeed;
            this.graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            this.graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

        }

        private void DrawTriangle(Triangle2D triangle, Triangle3D originalTriangle)
        {
            if (originalTriangle.Materialized)//originalTriangle.Material != null && originalTriangle.Material.Texture != null)
            {
                DrawTexturedTriangle(triangle, originalTriangle);
            }
            else
            {
                Vector3D centerPoint = (originalTriangle.Vertices[0] + originalTriangle.Vertices[1] + originalTriangle.Vertices[2]) * (1.0f / 3.0f);

                Color finalColor = CalculateLighting(centerPoint, originalTriangle.Normal, originalTriangle.Color);

                float shadowFactor = shadowMap.GetShadowFactor(centerPoint);
                float intensity = shadowFactor;

                finalColor = AdjustColorIntensity(finalColor, intensity);
                switch (GridMesh)
                {
                    case true:
                        using (Pen pen = new Pen(Color.Lime))
                        {
                            Point[] points = new Point[4];
                            Array.Copy(triangle.Vertices, points, 3);
                            points[3] = points[0]; // Schließe das Polygon
                            graphics.DrawLines(pen, points);
                        }
                        break;
                    case false:
                        using (SolidBrush brush = new SolidBrush(finalColor))
                        {
                            Point[] points = triangle.Vertices;
                            graphics.FillPolygon(brush, points);
                        }
                        break;
                }
            }
        }

        private void DrawTexturedTriangle(Triangle2D triangle, Triangle3D originalTriangle)
        {
            int quality = 4;
            float factor = MathExtensions.EnsureEven((int)Math.Round(quality - (quality * (Vector3D.Distance(camera.Position, (originalTriangle.Vertices[0] + originalTriangle.Vertices[1] + originalTriangle.Vertices[2]) / 3) / camera.FarClip))));
            Console.WriteLine(factor);

            Vector2D edge1 = new Vector2D(triangle.Vertices[1].X - triangle.Vertices[0].X, triangle.Vertices[1].Y - triangle.Vertices[0].Y);
            Vector2D edge2 = new Vector2D(triangle.Vertices[2].X - triangle.Vertices[0].X, triangle.Vertices[2].Y - triangle.Vertices[0].Y);
            float area = edge1.X * edge2.Y - edge1.Y * edge2.X;

            int minX = (int)Math.Floor(Math.Max(0, Math.Min(triangle.Vertices[0].X, Math.Min(triangle.Vertices[1].X, triangle.Vertices[2].X))) / factor);
            int maxX = (int)Math.Ceiling(Math.Min(width - 1, Math.Max(triangle.Vertices[0].X, Math.Max(triangle.Vertices[1].X, triangle.Vertices[2].X))) / factor);
            int minY = (int)Math.Floor(Math.Max(0, Math.Min(triangle.Vertices[0].Y, Math.Min(triangle.Vertices[1].Y, triangle.Vertices[2].Y))) / factor);
            int maxY = (int)Math.Ceiling(Math.Min(height - 1, Math.Max(triangle.Vertices[0].Y, Math.Max(triangle.Vertices[1].Y, triangle.Vertices[2].Y))) / factor);

            minX = Math.Max(0, minX);
            maxX = (int)Math.Min((width / factor) - 1, maxX);
            minY = Math.Max(0, minY);
            maxY = (int)Math.Min((height / factor) - 1, maxY);

            for (int y = minY; y <= maxY; y++)
            {
                bool wasInside = false;
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2D p = new Vector2D(x * factor - triangle.Vertices[0].X, y * factor - triangle.Vertices[0].Y);
                    float w1 = (p.X * edge2.Y - p.Y * edge2.X) / area;
                    float w2 = (p.Y * edge1.X - p.X * edge1.Y) / area;
                    float w3 = 1 - w1 - w2;

                    if (w1 >= 0 && w2 >= 0 && w3 >= 0)
                    {
                        wasInside = true;
                        Vector2D textureCoords = new Vector2D(
                            w1 * originalTriangle.TextureCoordinates[0].X + w2 * originalTriangle.TextureCoordinates[1].X + w3 * originalTriangle.TextureCoordinates[2].X,
                            w1 * originalTriangle.TextureCoordinates[0].Y + w2 * originalTriangle.TextureCoordinates[1].Y + w3 * originalTriangle.TextureCoordinates[2].Y
                        );

                        Color textureColor = originalTriangle.Material.GetColorAtUV(textureCoords.X, textureCoords.Y);
                        Vector3D normal = originalTriangle.Material.GetNormalAtUV(textureCoords.X, textureCoords.Y);

                        Vector3D worldPos = new Vector3D(
                            w1 * originalTriangle.Vertices[0].X + w2 * originalTriangle.Vertices[1].X + w3 * originalTriangle.Vertices[2].X,
                            w1 * originalTriangle.Vertices[0].Y + w2 * originalTriangle.Vertices[1].Y + w3 * originalTriangle.Vertices[2].Y,
                            w1 * originalTriangle.Vertices[0].Z + w2 * originalTriangle.Vertices[1].Z + w3 * originalTriangle.Vertices[2].Z
                        );

                        Color finalColor = CalculateLighting(worldPos, normal, textureColor);
                        float shadowFactor = shadowMap.GetShadowFactor(worldPos);
                        finalColor = AdjustColorIntensity(finalColor, shadowFactor);

                        try
                        {
                            graphics.FillRectangle(new SolidBrush(finalColor), x * factor, y * factor, factor, factor);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error setting pixel: {ex.Message}");
                        }
                    }
                    else if (wasInside)
                    {
                        break;
                    }
                }
            }

            try
            {
                graphics.DrawImageUnscaled(renderBuffer, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing image: {ex.Message}");
            }
        }






        private bool IsInsideTriangle(int x, int y, Triangle2D triangle)
        {
            // Implementierung der Punkt-in-Dreieck-Prüfung
            // Beispiel: Verwendung der Baryzentrische Koordinaten Methode
            Vector3D bary = CalculateBarycentricCoordinates(x, y, triangle);
            return bary.X >= 0 && bary.Y >= 0 && bary.Z >= 0;
        }

        private Vector3D CalculateBarycentricCoordinates(int x, int y, Triangle2D triangle)
        {
            float denominator = ((triangle.Vertices[1].Y - triangle.Vertices[2].Y) * (triangle.Vertices[0].X - triangle.Vertices[2].X) +
                                 (triangle.Vertices[2].X - triangle.Vertices[1].X) * (triangle.Vertices[0].Y - triangle.Vertices[2].Y));
            float a = ((triangle.Vertices[1].Y - triangle.Vertices[2].Y) * (x - triangle.Vertices[2].X) +
                       (triangle.Vertices[2].X - triangle.Vertices[1].X) * (y - triangle.Vertices[2].Y)) / denominator;
            float b = ((triangle.Vertices[2].Y - triangle.Vertices[0].Y) * (x - triangle.Vertices[2].X) +
                       (triangle.Vertices[0].X - triangle.Vertices[2].X) * (y - triangle.Vertices[2].Y)) / denominator;
            float c = 1 - a - b;
            return new Vector3D(a, b, c);
        }

        private Vector2D InterpolateTextureCoordinates(Vector3D barycentricCoords, Vector2D[] textureCoords)
        {
            return new Vector2D(
                barycentricCoords.X * textureCoords[0].X + barycentricCoords.Y * textureCoords[1].X + barycentricCoords.Z * textureCoords[2].X,
                barycentricCoords.X * textureCoords[0].Y + barycentricCoords.Y * textureCoords[1].Y + barycentricCoords.Z * textureCoords[2].Y
            );
        }

        private Vector3D InterpolateWorldPosition(Vector3D barycentricCoords, Vector3D[] vertices)
        {
            return new Vector3D(
                barycentricCoords.X * vertices[0].X + barycentricCoords.Y * vertices[1].X + barycentricCoords.Z * vertices[2].X,
                barycentricCoords.X * vertices[0].Y + barycentricCoords.Y * vertices[1].Y + barycentricCoords.Z * vertices[2].Y,
                barycentricCoords.X * vertices[0].Z + barycentricCoords.Y * vertices[1].Z + barycentricCoords.Z * vertices[2].Z
            );
        }
        private Color CalculateLighting(Vector3D point, Vector3D normal, Color TriangleColor)
        {
            Color baseColor = TriangleColor;
            Color finalColor = TriangleColor;
            float totalIntensity = Sun.AmbientLight; // Intensity of Shadows

            foreach (LightSource light in lightSources)
            {
                Vector3D lightDir = (light.Position - point).Normalize();
                float distance = (light.Position - point).Length();

                // Berechnung der Dämpfung basierend auf dem Leuchtradius
                float maxDistance = light.Radius * 2;
                float attenuation = light.Intensity * (1 - Math.Min(distance / maxDistance, 1)); // Härterer Rand durch Berücksichtigung des Radius
                float intensity = Math.Max(0, Vector3D.Dot(normal, lightDir)) * attenuation;

                // Härterer Übergang durch eine Exponentialfunktion
                float hardness = 2.0f; // Härtegrad
                float softIntensity = (float)Math.Pow(intensity, hardness);

                Color lightColor = BlendColors(baseColor, light.Color, softIntensity);

                finalColor = AddColors(finalColor, lightColor);

                totalIntensity += softIntensity;
            }

            totalIntensity = Math.Min(1f, totalIntensity);
            return AdjustColorIntensity(finalColor, totalIntensity);
        }

        private Color ApplyLightColor(Color baseColor, Color lightColor, float intensity)
        {
            float r = baseColor.R * lightColor.R / 255.0f * intensity;
            float g = baseColor.G * lightColor.G / 255.0f * intensity;
            float b = baseColor.B * lightColor.B / 255.0f * intensity;

            r = Math.Min(255, Math.Max(0, r));
            g = Math.Min(255, Math.Max(0, g));
            b = Math.Min(255, Math.Max(0, b));

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
        private Color AdjustColorIntensity(Color color, float intensity)
        {
            float r = color.R * intensity;
            float g = color.G * intensity;
            float b = color.B * intensity;
            r = (int)Math.Round(Math.Min(255, Math.Max(0, r)));
            g = (int)Math.Round(Math.Min(255, Math.Max(0, g)));
            b = (int)Math.Round(Math.Min(255, Math.Max(0, b)));

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private Color BlendColors(Color baseColor, Color lightColor, float intensity)
        {
            float r = (baseColor.R * (0 - intensity) + (lightColor.R) * intensity);
            float g = (baseColor.G * (0 - intensity) + (lightColor.G) * intensity);
            float b = (baseColor.B * (0 - intensity) + (lightColor.B) * intensity);

            r = (int)Math.Round(Math.Min(255, Math.Max(0, r)));
            g = (int)Math.Round(Math.Min(255, Math.Max(0, g)));
            b = (int)Math.Round(Math.Min(255, Math.Max(0, b)));


            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private Color AddColors(Color color1, Color color2)
        {
            int r = Math.Min(color1.R + color2.R, 255);
            int g = Math.Min(color1.G + color2.G, 255);
            int b = Math.Min(color1.B + color2.B, 255);
            return Color.FromArgb(r, g, b);
        }

        private void DrawLightSources()
        {
            foreach (var light in lightSources)
            {
                Vector3D lightScreenPos = camera.ProjectPointToScreen(light.Position, (int)graphics.VisibleClipBounds.Width, (int)graphics.VisibleClipBounds.Height);

                if (lightScreenPos != null)
                {
                    float imageSize = 20;
                    Rectangle imageRect = new Rectangle(
                        (int)(lightScreenPos.X - imageSize / 2),
                        (int)(lightScreenPos.Y - imageSize / 2),
                        (int)imageSize,
                        (int)imageSize
                    );
                    try {
                        graphics.DrawImage(lightBulbImage, imageRect);
                    }
                    catch (Exception e) { }
                }
            }
        }

        private void DrawSun()
        {
            Vector3D lightScreenPos = camera.ProjectPointToScreenUnristricted(Sun.Position, (int)graphics.VisibleClipBounds.Width, (int)graphics.VisibleClipBounds.Height);

            if (lightScreenPos != null)
            {
                float imageSize = 500;
                Rectangle imageRect = new Rectangle(
                    (int)(lightScreenPos.X - imageSize / 2),
                    (int)(lightScreenPos.Y - imageSize / 2),
                    (int)imageSize,
                    (int)imageSize
                );
                graphics.DrawImage(sunImage, imageRect);
                }
        }

        private void DrawSkybox()
        {
            // Bestimmen Sie die Größe der Sphäre (groß genug, um den gesamten Bereich der Kamera abzudecken)
            int width = (int)graphics.VisibleClipBounds.Width;
            int height = (int)graphics.VisibleClipBounds.Height * 2;
            int sphereRadius = Math.Max(width, height) * 2; // Sehr große Sphäre

            // Sphäre im Hintergrund zeichnen
            using (Bitmap skyboxBitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(skyboxBitmap))
                {
                    // Winkel der Kamera in Grad
                    float verticalAngle = camera.GetVerticalAngle();
                    float horizontalAngle = camera.GetHorizontalAngle();

                    // Sinus und Kosinus der Winkel berechnen, um die Verschiebung der Textur zu bestimmen
                    float verticalShift = (float)Math.Sin(verticalAngle * Math.PI / 180.0) * height / 2;
                    float horizontalShift = (float)Math.Sin(horizontalAngle * Math.PI / 180.0) * width / 2;

                    // Berechne die Positionen zum Zeichnen der Textur
                    int xOffset = (int)(horizontalShift % width);
                    int yOffset = (int)(verticalShift % height);

                    // Zeichne die Textur auf die gesamte Fläche, mit horizontaler und vertikaler Verschiebung
                    for (int x = -width; x < width * 2; x += width)
                    {
                        for (int y = -height; y < height * 2; y += height)
                        {
                            g.DrawImage(skyboxTexture, x + xOffset, y + yOffset, width, height);
                        }
                    }
                }

                // Zeichne das Bitmap auf den Bildschirm
                graphics.DrawImage(skyboxBitmap, 0, 0, width, height);
            }
        }


    }
}
