using System;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Physics
{
    public class PhysicsSDFScene : IScene<IPhysicsSDFObject>
    {
        private List<IPhysicsSDFObject> m_objects;

        public IReadOnlyCollection<IPhysicsSDFObject> Objects => m_objects;

        public PhysicsSDFScene()
        {
            m_objects = new List<IPhysicsSDFObject>();
        }

        public PhysicsSDFScene(IEnumerable<IPhysicsSDFObject> objects)
        {
            m_objects = new List<IPhysicsSDFObject>(objects);
        }

        public void AddObject(IPhysicsSDFObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (m_objects.Contains(obj))
                throw new ArgumentException($"{obj} has already contains in the scene");

            m_objects.Add(obj);
        }

        public bool RemoveObject(IPhysicsSDFObject obj)
        {
            return m_objects.Remove(obj);
        }

        public float SDF(Vector3 position)
        {
            float distance = float.MaxValue;

            foreach (IPhysicsSDFObject obj in m_objects)
            {
                float currentDistance = obj.SDF(position);

                if (currentDistance < distance)
                    distance = currentDistance;
            }

            return distance;
        }

        public Vector3 GetNormal(Vector3 position, float delta)
        {
            return new Vector3(
                SDF(position + Vector3.right   * delta) - SDF(position + Vector3.left * delta),
                SDF(position + Vector3.up      * delta) - SDF(position + Vector3.down * delta),
                SDF(position + Vector3.forward * delta) - SDF(position + Vector3.back * delta)
            ).normalized;
        }

        public bool RayMarch(Ray ray, out RaycastHit hit, float maxDistance, float iterations, float minDistance, float normalDelta)
        {
            hit = new RaycastHit();

            float depth = 0.0f;
            float dist;
            float depthWithMinDist = 0.0f;
            Vector3 position = ray.origin;

            for (int i = 0; i < iterations; i++)
            {
                position = ray.GetPoint(depth);

                dist = SDF(position);

                if (dist >= maxDistance)
                    return false;

                depth += dist;

                if (dist <= minDistance)
                {
                    hit.distance = depth;
                    hit.point = position;
                    hit.normal = GetNormal(position, normalDelta);
                    return true;
                }

                if (minDistance > dist)
                {
                    minDistance = dist;
                    depthWithMinDist = depth;
                }
            }

            hit.distance = depthWithMinDist;
            hit.point = position;
            hit.normal = GetNormal(position, normalDelta);
            return true;
        }
    }
}