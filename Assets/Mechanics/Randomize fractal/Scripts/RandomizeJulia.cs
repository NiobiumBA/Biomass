using RayMarching.Fractals;
using UnityEngine;
using static RayMarching.Fractals.JuliaSetAsset;

public class RandomizerJulia : RandomizerFractal
{
    [SerializeField] private JuliaSetAsset m_minValues;
    [SerializeField] private JuliaSetAsset m_maxValues;

    public override FractalAsset RandomFractalAsset
    {
        get
        {
            JuliaSetAsset result = ScriptableObject.CreateInstance<JuliaSetAsset>();

            result.Scale = Random.Range(m_minValues.Scale, m_maxValues.Scale);
            result.Iterations = Random.Range(m_minValues.Iterations, m_maxValues.Iterations);
            result.Power = GetRandomIntOrFloat(m_minValues.Power, m_maxValues.Power);

            result.AdditiveConstant = GetRandomHyperComplex(m_minValues.AdditiveConstant, m_maxValues.AdditiveConstant);

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

    private HyperComplex GetRandomHyperComplex(HyperComplex min, HyperComplex max)
    {
        float radius = Random.Range(min.Radius, max.Radius);
        float angle1 = Random.Range(0, 2 * Mathf.PI);
        float angle2 = Random.Range(0, 2 * Mathf.PI);

        return new HyperComplex(radius, angle1, angle2);
    }

    private float GetRandomIntOrFloat(float min, float max)
    {
        bool isInt = Random.Range(0, 2) == 0;

        if (isInt)
            return Random.Range(Mathf.FloorToInt(min), Mathf.CeilToInt(max));
        else
            return Random.Range(min, max);
    }
}
