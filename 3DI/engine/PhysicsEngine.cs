using System;
using System.Collections.Generic;

namespace Simple3DEngine
{
    public class PhysicsEngine
    {
        private const float RepulsionForce = 0.1f;
        private const float CollisionThreshold = 0.01f;

        public void ProcessCollisions(List<Object3D> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    if (objects[i].EnableCollision && objects[j].EnableCollision)
                    {
                        if (CheckCollision(objects[i], objects[j]))
                        {
                            HandleCollision(objects[i], objects[j]);
                        }
                    }
                }
            }
        }

        public bool CheckCollision(Object3D obj1, Object3D obj2)
        {
            if ((obj1.BoundingBoxes == null && obj2.BoundingBoxes == null) || (!obj1.EnableCollision || !obj2.EnableCollision))
                return false;

            foreach (var box1 in obj1.BoundingBoxes)
            {
                foreach (var box2 in obj2.BoundingBoxes)
                {
                    if (CheckBoxCollision(box1, box2, obj1.Position, obj2.Position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckBoxCollision(BoundingBox box1, BoundingBox box2, Vector3D pos1, Vector3D pos2)
        {
            Vector3D min1 = box1.Min + pos1;
            Vector3D max1 = box1.Max + pos1;
            Vector3D min2 = box2.Min + pos2;
            Vector3D max2 = box2.Max + pos2;

            return (min1.X <= max2.X && max1.X >= min2.X &&
                    min1.Y <= max2.Y && max1.Y >= min2.Y &&
                    min1.Z <= max2.Z && max1.Z >= min2.Z);
        }

        private void HandleCollision(Object3D obj1, Object3D obj2)
        {
            if (obj1.IsStatic && obj2.IsStatic)
                return;

            Vector3D collisionNormal = (obj2.Position - obj1.Position).Normalize();
            float totalMass = obj1.Mass + obj2.Mass;

            if (!obj1.IsStatic && !obj2.IsStatic)
            {
                float obj1Force = (obj2.Mass / totalMass) * RepulsionForce;
                float obj2Force = (obj1.Mass / totalMass) * RepulsionForce;

                obj1.Move(collisionNormal * -obj1Force);
                obj2.Move(collisionNormal * obj2Force);
            }
            else if (!obj1.IsStatic)
            {
                obj1.Move(collisionNormal * -RepulsionForce);
            }
            else if (!obj2.IsStatic)
            {
                obj2.Move(collisionNormal * RepulsionForce);
            }
        }

        public bool WouldCollide(Object3D obj, List<Object3D> otherObjects, Vector3D newPosition)
        {
            Vector3D originalPos = obj.Position;
            obj.Position = newPosition;

            bool wouldCollide = false;
            foreach (var otherObj in otherObjects)
            {
                if (obj != otherObj && CheckCollision(obj, otherObj))
                {
                    wouldCollide = true;
                    break;
                }
            }

            obj.Position = originalPos;
            return wouldCollide;
        }
    }
}