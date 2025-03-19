using RayMarching.Fractals;
using UnityEngine;

public class KleinianRandomizer : RandomizerFractal
{
    [SerializeField] private PseudoKleinianAsset m_minValues;
    [SerializeField] private PseudoKleinianAsset m_maxValues;

    public override FractalAsset RandomFractalAsset
    {
        get
        {
            PseudoKleinianAsset result = ScriptableObject.CreateInstance<PseudoKleinianAsset>();

            result.Scale = Random.Range(m_minValues.Scale, m_maxValues.Scale);

            result.Size = Random.Range(m_minValues.Size, m_maxValues.Size);
            result.CSize = GetRandomVector(m_minValues.CSize, m_maxValues.CSize);
            result.C = GetRandomVector(m_minValues.C, m_maxValues.CSize);

            result.Iterations = Random.Range(m_minValues.Iterations, m_maxValues.Iterations);
            result.ColorDifference = Random.Range(m_minValues.ColorDifference, m_maxValues.ColorDifference);

            Color firstColor = Random.ColorHSV();
            firstColor.a = 1;
            result.FirstColor = firstColor;
            Color secondColor = Random.ColorHSV();
            secondColor.a = 1;
            result.SecondColor = secondColor;

            return result;
        }
    }

    private Vector3 GetRandomVector(Vector3 a, Vector3 b)
    {
        return new Vector3(Random.Range(a.x, b.x),
                           Random.Range(a.y, b.y),
                           Random.Range(a.z, b.z));
    }
}
