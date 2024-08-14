using Simple3DEngine;
using System;
using System.Collections.Generic;

public class Object3D
{
    public Vector3D Position;
    public Vector3D Rotation;
    public List<Triangle3D> Triangles;
    public List<BoundingBox> BoundingBoxes;
    public bool EnableCulling = true;
    public bool EnableCollision = false;
    public bool IsStatic { get; set; } = false;
    public float Mass { get; set; } = 1.0f;

    PhysicsEngine physicsEngine = new PhysicsEngine();

    public Object3D(Vector3D position)
    {
        BoundingBoxes = new List<BoundingBox>();
        Position = position;
        Rotation = new Vector3D(0, 0, 0);
        Triangles = new List<Triangle3D>();
    }

    public void Move(Vector3D direction)
    {
        if (!IsStatic)
        {
            Position += direction;
        }
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
        if (!IsStatic)
        {
            Vector3D leftDirection = GetLocalLeftVector();
            Move(leftDirection * distance);
        }
    }

    public void MoveFromLocalPositionRight(float distance)
    {
        if (!IsStatic)
        {
            Vector3D rightDirection = GetLocalRightVector();
            Move(rightDirection * distance);
        }
    }

    public void MoveFromLocalPositionForward(float distance)
    {
        if (!IsStatic)
        {
            Vector3D forwardDirection = GetLocalForwardVector();
            Move(forwardDirection * distance);
        }
    }

    public void MoveFromLocalPositionBackward(float distance)
    {
        if (!IsStatic)
        {
            Vector3D backwardDirection = GetLocalBackwardVector();
            Move(backwardDirection * distance);
        }
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

    private Vector3D GetLocalBackwardVector()
    {
        return -GetLocalForwardVector();
    }
}