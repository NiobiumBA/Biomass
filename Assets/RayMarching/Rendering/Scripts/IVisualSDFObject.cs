using System.Collections.Generic;

namespace RayMarching.Rendering
{
    public interface IVisualSDFObject
    {
        IReadOnlyCollection<string> ShaderKeywords { get; }

        void ApplyContext(ISDFRendererContext context);
    }
}