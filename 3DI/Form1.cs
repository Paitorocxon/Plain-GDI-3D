using Simple3DEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace _3DI
{
    public partial class Form1 : Form
    {



        int ww = 800;
        int wh = 600;
        
        private List<Object3D> objects;
        private Camera camera;
        private Bitmap renderBuffer;
        private Graphics graphics;
        private Renderer renderer;
        private List<LightSource> lightSources;
        private List<AudioSource3D> audioSources;

        // Maussteuerung
        private bool isDragging = false;
        private Point lastMousePosition;
        AudioSource3D audioSource;

        float moveSpeed = 0.5f;

        public Form1()
        {
            InitializeComponent();




            this.DoubleBuffered = true;
            this.Size = new Size(ww, wh);
            //this.FormBorderStyle = FormBorderStyle.None;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;

            renderBuffer = new Bitmap(ww, wh);
            graphics = Graphics.FromImage(renderBuffer);

            objects = new List<Object3D>();

            // Beispiel: Initialisierung der Kamera in Form1.cs
            float aspectRatio = (float)wh / ww;
            camera = new Camera(
                new Vector3D(0, 1, 0), // Position der Kamera
                new Vector3D(0, 0, 0),    // Richtung der Kamera (nach vorne)
                90,                       // Field of View
                ww,                     // Width
                wh,                      // Height
                0.1f,                     // Near Clip Plane
                100.0f                    // Far Clip Plane
            );

            lightSources = new List<LightSource>
            { };
            audioSources = new List<AudioSource3D>
            { };

            Color[] colors = new Color[]
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Magenta,
                Color.Cyan,
                Color.Yellow
            };


            LightSource sun = new LightSource(new Vector3D(0, 0, 0), Color.FromArgb(255, 255, 200), 10);
            lightSources.Add(sun);
            float radius = 10.0f;
            // Anzahl der Lichtquellen
            int numLights = 6;
            // Mittelpunkt des Kreises
            Vector3D center = new Vector3D(-20, -20, -20);
            for (int i = 0; i < numLights; i++)
            {
                // Berechne den Winkel für jede Lichtquelle
                float angle = (float)(i * 2 * Math.PI / numLights);
                // Berechne die Position auf dem Kreis
                float x = center.X + radius * (float)Math.Cos(angle);
                float z = center.Z + radius * (float)Math.Sin(angle);
                // Erstelle die Lichtquelle
                Vector3D position = new Vector3D(x, center.Y, z);
                Color color = colors[i];
                LightSource light = new LightSource(position, color, 5.0f, 10);
                lightSources.Add(light);
            }

            audioSource = new AudioSource3D(
                new Vector3D(0, 0, 0),     // Position der Audioquelle
                5f,                      // Lautstärke
                100.0f,                    // Maximaler Hörbereich
                0.0f,                     // Minimaler Hörbereich
                @"C:\users\Fabian\Downloads\TTM, APOVABIN, LOWX - EYES DONT LIE.mp3" // Pfad zur Audiodatei
            );

            //audioSource.Play();
            //audioSource.Update(camera.Position, camera.Direction);
            renderer = new Renderer(graphics, camera, lightSources, "lightbulb.png");




            // Beispiele für das Hinzufügen von Objekten
            //HeightMap terrain = new HeightMap(new Vector3D(-40, -40, -40), 40, 40, 20, 2f);
            //objects.Add(terrain);

            for (int x = 0; x < 20; x++)
            {
                // Die maximale Anzahl an Blöcken in der Y-Achse für die aktuelle Stufe ist x + 1
                for (int y = 0; y <= x; y++)
                {
                    // Erstellen eines Blocks an der Position (x, -10 + y, y)
                    Cube c = new Cube(new Vector3D(x, -10 + y, y), 1);
                    c.EnableCollision = true;
                    c.IsStatic = true;
                    objects.Add(c);
                }
            }


            Cube cameraCube = new Cube(new Vector3D(0, 1, 0), 2);
            cameraCube.EnableCollision = true;

            Cube cube = new Cube(new Vector3D(0, 1, 0), 2);
            cube.EnableCollision = true;
            cube.IsStatic = true;
            objects.Add(cameraCube);
            objects.Add(cube);

            HeightMap worldborder = new HeightMap(new Vector3D(-40, -40, -40), 20, 20, 3, 0.5f);
            worldborder.EnableCollision = true;
            worldborder.IsStatic = false;
            objects.Add(worldborder);

            Cube cubeColl = new Cube(new Vector3D(0, 1, 0), 2);
            cubeColl.EnableCollision = true;
            cubeColl.IsStatic = true;
            objects.Add(cubeColl);


            camera.RotateHorizontal(-89.5f);

            Timer timer = new Timer();
            timer.Interval = 1; // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();

            // Event-Handler für Mausereignisse und Tastatureingaben registrieren
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.KeyDown += Form1_KeyDown;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Aktualisiere die Lautstärke und das Stereo-Panorama basierend auf der Kamera-Position und -Richtung

            //renderer.Sun.Intensity = 1.2f;
            //renderer.Sun.AmbientLight = 0.5f;
            //objects[1].RotateZ(0.2f);
            //camera.Position -= new Vector3D(0,1,0) * 0.0098f;
            renderer.Sun.AmbientLightColor = Color.FromArgb(255, 255, 255);
            renderer.CullingTolerance = 0.01f;
            //renderer.Render(objects);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //audioSource.Update(camera.Position,camera.Direction);

            PhysicsEngine pe = new PhysicsEngine();
            if (!pe.WouldCollide(objects[0], objects, camera.Position - new Vector3D(0, 1, 0)))
            {
                camera.Position -= new Vector3D(0, 1, 0) * (moveSpeed / 2);
                objects[0].Position = camera.Position;
            }


            renderer.Render(objects);
            camera.Position = objects[0].Position;
            e.Graphics.DrawImage(renderBuffer, 0, 0);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            // Bewegungssteuerung
            if (e.KeyCode == Keys.W) camera.Position += new Vector3D(0, 0.25f, 0) + new Vector3D(camera.Direction.X, 0f, camera.Direction.Z) * moveSpeed; ;//camera.Direction * moveSpeed;
            if (e.KeyCode == Keys.S) camera.Position -= camera.Direction * moveSpeed;
            if (e.KeyCode == Keys.D) camera.Position -= Vector3D.CrossProduct(camera.Direction, camera.Up) * moveSpeed;
            if (e.KeyCode == Keys.A) camera.Position += Vector3D.CrossProduct(camera.Direction, camera.Up) * moveSpeed;
            if (e.KeyCode == Keys.Space) camera.Position += new Vector3D(0, 1, 0) * moveSpeed;
            if (e.KeyCode == Keys.ControlKey) camera.Position -= new Vector3D(0, 1, 0) * moveSpeed;
            if (e.KeyCode == Keys.K) objects[0].Move(new Vector3D(0, -1, 0) * 0.2f);

            if (e.KeyCode == Keys.Q)
            {
                lightSources[0].Intensity += 1;
            }

            if (e.KeyCode == Keys.E)
            {
                lightSources[0].Intensity -= 1;
            }

            // Rotationssteuerung
            if (e.KeyCode == Keys.Left) camera.RotateHorizontal(-0.05f);
            if (e.KeyCode == Keys.Right) camera.RotateHorizontal(0.05f);
            if (e.KeyCode == Keys.Up) camera.RotateVertical(-0.05f);
            if (e.KeyCode == Keys.Down) camera.RotateVertical(0.05f);

            // Aspect Ratio Anpassung
            if (e.KeyCode == Keys.F1) camera.FOV = 90;
            if (e.KeyCode == Keys.F2) camera.FOV = 60;
            if (e.KeyCode == Keys.F3) renderer.ToLowQuality();
            if (e.KeyCode == Keys.F4) renderer.ToHighQuality();

            lightSources[0].Position = camera.Position;

            this.Invalidate(); // Formular neu zeichnen
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                float deltaX = e.X - lastMousePosition.X;
                float deltaY = e.Y - lastMousePosition.Y;

                const float sensitivity = 0.01f; // Sensitivität der Maus
                camera.RotateVertical(-deltaY * sensitivity);
                camera.RotateHorizontal(-deltaX * sensitivity);

                lastMousePosition = e.Location;
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
    }
}
