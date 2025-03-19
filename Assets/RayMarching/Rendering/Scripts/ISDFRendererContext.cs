using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Rendering
{
    public interface ISDFRendererContext
    {
        IReadOnlyCollection<string> EnabledKeywords { get; }

        void EnableKeyword(string keyword);
        void DisableKeyword(string keyword);

        void SetInteger(string varName, int value);
        void SetBool(string varName, bool value);
        void SetFloat(string varName, float value);
        void SetColor(string varName, Color color);
        void SetVector(string varName, Vector3 value);
        void SetMatrix(string varName, Matrix4x4 value);
        void SetTexture(string varName, Texture value);
    }
}
