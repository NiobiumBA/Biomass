using RayMarching.Rendering;
using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Fractals
{
    public abstract class FractalObject<TAsset> : ISDFObject
                where TAsset : FractalAsset
    {
        public abstract IReadOnlyCollection<string> ShaderKeywords { get; }
        public TAsset Asset { get; private set; }

        protected FractalObject(TAsset asset)
        {
            Asset = asset;
        }

        public abstract void ApplyContext(ISDFRendererContext context);

        public abstract float SDF(Vector3 position);
    }
}
