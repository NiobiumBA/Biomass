using RayMarching.Rendering;
using UnityEngine;

namespace RayMarching
{
    public static class RayMarchingUtils
    {
        public static float GetMinDistanceFactor(Vector2 textureSize)
        {
            float verticalMinDist = 1.0f / textureSize.x;
            float horizontalMinDist = 1.0f / textureSize.y;
            return Mathf.Min(verticalMinDist, horizontalMinDist);
        }

        public static Matrix4x4 GetViewportToWorldMatrix(Camera camera)
        {
            return (camera.projectionMatrix * camera.worldToCameraMatrix).inverse;
        }

        public static void DisableKeywords(ISDFRendererContext context)
        {
            foreach (string keyword in context.EnabledKeywords)
            {
                context.DisableKeyword(keyword);
            }
        }
    }
}