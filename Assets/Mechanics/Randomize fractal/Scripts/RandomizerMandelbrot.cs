using RayMarching.Fractals;
using UnityEngine;

public class RandomizerMandelbrot : RandomizerFractal
{
    [SerializeField] private MandelbrotAsset m_minValues;
    [SerializeField] private MandelbrotAsset m_maxValues;

    public override FractalAsset RandomFractalAsset
    {
        get
        {
            MandelbrotAsset result = ScriptableObject.CreateInstance<MandelbrotAsset>();

            result.Scale = Random.Range(m_minValues.Scale, m_maxValues.Scale);

            bool integerPower = Random.Range(0, 2) == 0;

            float power;

            if (integerPower)
                power = Random.Range(Mathf.FloorToInt(m_minValues.Power), Mathf.CeilToInt(m_maxValues.Power));
            else
                power = Random.Range(m_minValues.Power, m_maxValues.Power);
            
            result.Power = power;
            
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
}
