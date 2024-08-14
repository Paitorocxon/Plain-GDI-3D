using _3DI.engine;
using System;
using System.Drawing;

namespace Simple3DEngine
{
    public class Camera
    {
        public Vector3D Position { get; set; }
        public Vector3D Direction { get; private set; }
        public Vector3D Up { get; private set; }
        public float FOV { get; set; } // Field of View in degrees
        public float AspectRatio { get; private set; }
        public float NearClip { get; set; }
        public float FarClip { get; set; }
        public Vector3D Rotation { get; private set; } // Rotation in radians
        private Vector2D Resolution { get; set; }

        public Camera(Vector3D position, Vector3D direction, float fov, int width, int height, float nearClip, float farClip)
        {
            Position = position;
            Direction = direction.Normalize();
            FOV = fov;
            SetResolution(width, height); // Set AspectRatio based on resolution
            Resolution = new Vector2D(width, height);
            NearClip = nearClip;
            FarClip = farClip;
            Rotation = new Vector3D(0, 0, 0); // Initial rotation
            UpdateCameraVectors(); // Initialize Direction and Up
        }

        public void SetResolution(int width, int height)
        {
            AspectRatio = (float)width / height;
        }

        public Vector2D GetResolution()
        {
            return Resolution;
        }

        public void RotateHorizontal(float angle)
        {
            Rotation.Y += angle;
            Rotation.Y %= (float)(2 * Math.PI);
            UpdateCameraVectors();
        }

        public void RotateVertical(float angle)
        {
            Rotation.X += angle;
            Rotation.X = (float)Math.Max(-Math.PI / 2, Math.Min(Math.PI / 2, Rotation.X));
            UpdateCameraVectors();
        }

        private void UpdateCameraVectors()
        {
            float pitch = Rotation.X;
            float yaw = Rotation.Y;

            // Berechne die neue Richtung der Kamera
            float cosPitch = (float)Math.Cos(pitch);
            float sinPitch = (float)Math.Sin(pitch);
            float cosYaw = (float)Math.Cos(yaw);
            float sinYaw = (float)Math.Sin(yaw);

            Direction = new Vector3D(
                cosPitch * cosYaw,
                sinPitch,
                cosPitch * sinYaw
            ).Normalize();

            // Up-Vektor neu berechnen
            Vector3D right = Vector3D.CrossProduct(Direction, new Vector3D(0, 1, 0)).Normalize();
            Up = Vector3D.CrossProduct(right, Direction).Normalize();
        }

        public Triangle2D ProjectTriangle(Triangle3D triangle, int width, int height)
        {
            var screenVertices = new Point[3];

            for (int i = 0; i < 3; i++)
            {
                Vector3D screenPoint = ProjectPointToScreen(triangle.Vertices[i], width, height);
                if (screenPoint == null)
                {
                    return null; // Objekt außerhalb des Sichtfelds
                }
                screenVertices[i] = new Point((int)screenPoint.X, (int)screenPoint.Y);
            }

            return new Triangle2D(screenVertices[0], screenVertices[1], screenVertices[2], triangle.Normal);
        }

        private Vector3D RotateVector(Vector3D vector, Vector3D rotation)
        {
            float cosX = (float)Math.Cos(rotation.X);
            float sinX = (float)Math.Sin(rotation.X);
            float cosY = (float)Math.Cos(rotation.Y);
            float sinY = (float)Math.Sin(rotation.Y);
            float cosZ = (float)Math.Cos(rotation.Z);
            float sinZ = (float)Math.Sin(rotation.Z);

            // Rotation um die Z-Achse
            float x = vector.X * cosZ - vector.Y * sinZ;
            float y = vector.X * sinZ + vector.Y * cosZ;
            float z = vector.Z;

            // Rotation um die Y-Achse
            float x1 = x * cosY + z * sinY;
            float z1 = -x * sinY + z * cosY;

            // Rotation um die X-Achse
            float y1 = y * cosX - z1 * sinX;
            z1 = y * sinX + z1 * cosX;

            return new Vector3D(x1, y1, z1);
        }

        public void DrawAxes(Graphics g, int width, int height)
        {
            // Zeichne die Achsen der Kamera
        }
        public Vector3D ProjectPointToScreen(Vector3D point, int width, int height)
        {
            Vector3D toVertex = point - Position;

            // Transformiere den Punkt in den Kamera-Raum
            Vector3D cameraSpace = TransformToCameraSpace(toVertex);

            if (cameraSpace.Z <= 0)
            {
                return null; // Punkt ist hinter der Kamera
            }

            // Perspektivische Projektion
            float fovRad = FOV * (float)Math.PI / 180.0f;
            float tanHalfFOV = (float)Math.Tan(fovRad * 0.5f);
            float near = NearClip;

            float x = cameraSpace.X / (cameraSpace.Z * tanHalfFOV);
            float y = cameraSpace.Y / (cameraSpace.Z * tanHalfFOV);

            // Normalisierung auf den Bildschirmbereich
            float screenX = (x + 1) * 0.5f * width;
            float screenY = (1 - y) * 0.5f * height;

            return new Vector3D(screenX, screenY, cameraSpace.Z);
        }

        public Vector3D ProjectPointToScreenUnristricted(Vector3D point, int width, int height)
        {
            Vector3D toVertex = point - Position;
            toVertex = RotateVector(toVertex, Rotation);

            float z = toVertex.Z;


            float fovRad = FOV * (float)Math.PI / 180.0f; // Convert FOV to radians
            float scale = (float)(1.0 / Math.Tan(fovRad / 2)); // Perspective scale based on FOV

            float x = toVertex.X * scale / z;
            float y = toVertex.Y * scale / z;

            return new Vector3D(
                (x * AspectRatio + 1) * 0.5f * width,
                (1 - y) * 0.5f * height,
                0 // Z-Koordinate nicht benötigt für 2D-Projektion
            );
        }
        public float GetHorizontalAngle()
        {
            // Konvertiere den Yaw-Winkel von Bogenmaß in Grad
            return Rotation.Y * (180.0f / (float)Math.PI);
        }
        public float GetVerticalAngle()
        {
            // Konvertiere den Pitch-Winkel von Bogenmaß in Grad
            return Rotation.X * (180.0f / (float)Math.PI);
        }
        private Vector3D TransformToCameraSpace(Vector3D point)
        {
            // Berechne die Kamera-Basis-Vektoren
            Vector3D forward = Direction;
            Vector3D right = Vector3D.CrossProduct(Up, forward).Normalize();
            Vector3D up = Vector3D.CrossProduct(forward, right);

            // Transformiere den Punkt in den Kamera-Raum
            return new Vector3D(
                Vector3D.Dot(point, right),
                Vector3D.Dot(point, up),
                Vector3D.Dot(point, forward)
            );
        }



        public void Move(Vector3D direction)
        {
            Position += direction;
        }

        public void RotateX(float angle)
        {
            Rotation.X += angle;
        }

        public void RotateY(float angle)
        {
            Rotation.Y += angle;
        }

        public void RotateZ(float angle)
        {
            Rotation.Z += angle;
        }

        public void MoveFromLocalPositionLeft(float distance)
        {
            Vector3D leftDirection = GetLocalLeftVector();
            Move(leftDirection * distance);
        }

        public void MoveFromLocalPositionRight(float distance)
        {
            Vector3D rightDirection = GetLocalRightVector();
            Move(rightDirection * distance);
        }

        public void MoveFromLocalPositionForward(float distance)
        {
            Vector3D forwardDirection = GetLocalForwardVector();
            Move(forwardDirection * distance);
            
        }

        public void MoveFromLocalPositionBackward(float distance)
        {
            Vector3D backwardDirection = GetLocalBackwardVector();
            Move(backwardDirection * distance);
            
        }

        private Vector3D GetLocalRightVector()
        {
            float yawRadians = Rotation.Y * (float)Math.PI / 180f;
            return new Vector3D((float)Math.Sin(yawRadians), 0, (float)Math.Cos(yawRadians));
        }

        private Vector3D GetLocalLeftVector()
        {
            return -GetLocalRightVector();
        }

        private Vector3D GetLocalForwardVector()
        {
            float yawRadians = Rotation.Y * (float)Math.PI / 180f;
            return new Vector3D((float)Math.Sin(yawRadians - Math.PI / 2), 0, (float)Math.Cos(yawRadians - Math.PI / 2));
        }

        /*
         *         
         private Vector3D GetLocalForwardVector()
        {
            float yaw = Rotation.Y;
            float pitch = Rotation.X;
            return new Vector3D(
                (float)Math.Sin(yaw),
                -(float)Math.Sin(pitch),
                (float)Math.Cos(yaw)
            ).Normalize();
        }
        
         */

        private Vector3D GetLocalBackwardVector()
        {
            return -GetLocalForwardVector();
        }
        public override string ToString()
        {
            return Position.ToString() + "/" + Direction.ToString() + "/" + Rotation.ToString();
        }

    }
}
