// Datei: EngineForm.cs
using Simple3DEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3DI
{
    public abstract class EngineForm : Form
    {
        protected int ww = 1200;
        protected int wh = 900;

        protected List<Object3D> objects;
        protected Camera camera;
        protected Bitmap renderBuffer;
        protected Graphics graphics;
        protected Renderer renderer;
        protected List<LightSource> lightSources;
        protected List<AudioSource3D> audioSources;

        private bool isDragging = false;
        private Point lastMousePosition;

        public EngineForm()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(ww, wh);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            renderBuffer = new Bitmap(ww, wh);
            graphics = Graphics.FromImage(renderBuffer);

            objects = new List<Object3D>();
            lightSources = new List<LightSource>();
            audioSources = new List<AudioSource3D>();

            float aspectRatio = (float)wh / ww;
            camera = new Camera(
                new Vector3D(0, 0, 0), // Position der Kamera
                new Vector3D(0, 0, 0), // Richtung der Kamera (nach vorne)
                90,                    // Field of View
                ww,                    // Width
                wh,                    // Height
                0.1f,                  // Near Clip Plane
                100.0f                 // Far Clip Plane
            );

            renderer = new Renderer(graphics, camera, lightSources, "lightbulb.png");

            Timer timer = new Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += Timer_Tick;
            timer.Start();

            this.MouseDown += EngineForm_MouseDown;
            this.MouseMove += EngineForm_MouseMove;
            this.MouseUp += EngineForm_MouseUp;
            this.KeyDown += EngineForm_KeyDown;
        }

        protected virtual void Timer_Tick(object sender, EventArgs e)
        {
            renderer.Sun.Intensity = 1.2f;
            renderer.Sun.AmbientLight = 0.5f;
            renderer.Sun.AmbientLightColor = Color.FromArgb(255, 255, 255);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            renderer.Render(objects);
            e.Graphics.DrawImage(renderBuffer, 0, 0);
        }

        protected virtual void EngineForm_KeyDown(object sender, KeyEventArgs e)
        {
            float moveSpeed = 0.5f;
            if (e.KeyCode == Keys.W) camera.Position += Vector3D.CrossProduct(camera.Direction, camera.Up) * moveSpeed;
            if (e.KeyCode == Keys.S) camera.Position -= Vector3D.CrossProduct(camera.Direction, camera.Up) * moveSpeed;
            if (e.KeyCode == Keys.Space) camera.Position += new Vector3D(0, 1, 0) * moveSpeed;
            if (e.KeyCode == Keys.ControlKey) camera.Position -= new Vector3D(0, 1, 0) * moveSpeed;

            if (e.KeyCode == Keys.Q) lightSources[0].Intensity += 1;
            if (e.KeyCode == Keys.E) lightSources[0].Intensity -= 1;

            if (e.KeyCode == Keys.Left) camera.RotateHorizontal(-0.05f);
            if (e.KeyCode == Keys.Right) camera.RotateHorizontal(0.05f);
            if (e.KeyCode == Keys.Up) camera.RotateVertical(-0.05f);
            if (e.KeyCode == Keys.Down) camera.RotateVertical(0.05f);

            if (e.KeyCode == Keys.F1) camera.FOV = 90;
            if (e.KeyCode == Keys.F2) camera.FOV = 60;
            if (e.KeyCode == Keys.F3) renderer.ToLowQuality();
            if (e.KeyCode == Keys.F4) renderer.ToHighQuality();

            lightSources[0].Position = camera.Position;
            this.Invalidate();
        }

        protected virtual void EngineForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
            }
        }

        protected virtual void EngineForm_MouseMove(object sender, MouseEventArgs e)
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

        protected virtual void EngineForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
    }
}
