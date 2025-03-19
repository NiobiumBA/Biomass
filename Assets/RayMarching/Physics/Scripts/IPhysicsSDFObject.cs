using UnityEngine;

namespace RayMarching.Physics
{
    public interface IPhysicsSDFObject
    {
        float SDF(Vector3 position);
    }
}