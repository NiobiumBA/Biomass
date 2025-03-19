using UnityEngine;

namespace RayMarching.Fractals
{
    public abstract class FractalAsset : ScriptableObject
    {
        public abstract ISDFObject SDFObject { get; }
        public abstract float Scale { get; set; }
    }
}