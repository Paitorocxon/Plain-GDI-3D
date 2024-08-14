namespace Simple3DEngine
{
    public class Cube : Object3D
    {
        public Cube(Vector3D position, float size) : base(position)
        {
            float halfSize = size / 2;
            this.BoundingBoxes.Add(new BoundingBox(new Vector3D(-halfSize, -halfSize, -halfSize), new Vector3D(halfSize, halfSize, halfSize)));
            // Front face
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, -halfSize, halfSize),
                new Vector3D(halfSize, -halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, -halfSize, halfSize),
                new Vector3D(halfSize, halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, halfSize)
            ));

            // Back face
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, -halfSize, -halfSize),
                new Vector3D(-halfSize, -halfSize, -halfSize),
                new Vector3D(halfSize, halfSize, -halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, -halfSize, -halfSize),
                new Vector3D(-halfSize, halfSize, -halfSize),
                new Vector3D(halfSize, halfSize, -halfSize)
            ));

            // Left face
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, -halfSize, -halfSize),
                new Vector3D(-halfSize, -halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, -halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, -halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, -halfSize)
            ));

            // Right face
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, -halfSize, halfSize),
                new Vector3D(halfSize, -halfSize, -halfSize),
                new Vector3D(halfSize, halfSize, halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, -halfSize, -halfSize),
                new Vector3D(halfSize, halfSize, -halfSize),
                new Vector3D(halfSize, halfSize, halfSize)
            ));

            // Top face
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, halfSize, halfSize),
                new Vector3D(halfSize, halfSize, halfSize),
                new Vector3D(-halfSize, halfSize, -halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, halfSize, halfSize),
                new Vector3D(halfSize, halfSize, -halfSize),
                new Vector3D(-halfSize, halfSize, -halfSize)
            ));

            // Bottom face
            Triangles.Add(new Triangle3D(
                new Vector3D(-halfSize, -halfSize, -halfSize),
                new Vector3D(halfSize, -halfSize, -halfSize),
                new Vector3D(-halfSize, -halfSize, halfSize)
            ));
            Triangles.Add(new Triangle3D(
                new Vector3D(halfSize, -halfSize, -halfSize),
                new Vector3D(halfSize, -halfSize, halfSize),
                new Vector3D(-halfSize, -halfSize, halfSize)
            ));
        }
    }
}