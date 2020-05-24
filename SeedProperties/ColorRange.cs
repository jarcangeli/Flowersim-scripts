using UnityEngine;
using Random = System.Random;

[System.Serializable]
public struct ColorRange
{
    public Color min;
    public Color max;
    private Gradient gradient;

    public ColorRange(Color _min, Color _max)
    {
        Color.RGBToHSV(_min, out float minH, out float _, out float _);
        Color.RGBToHSV(_max, out float maxH, out float _, out float _);
        if (maxH - minH < 0.5f && maxH - minH > 0f)
        {
            min = _min;
            max = _max;
        }
        else // dont go wrong way around colour wheel
        {
            min = _max;
            max = _min;
        }
        gradient = CreateColorGradient(min, max);
        }

    public Color GetValue()
    {
        // get a value within the range
        return gradient.Evaluate(UnityEngine.Random.value);
    }
    public Color GetValue(Random rand)
    {
        // get a value within the range
        float val = (float)rand.NextDouble();
        val = val / 3f + 0.33f; // favour middle
        return gradient.Evaluate(val);
    }
    public Color GetValue(float val)
    {
        // get a value within the range
        return gradient.Evaluate(val);
    }
    public static ColorRange Combine(ColorRange a, ColorRange b)
    {
        return new ColorRange(a.GetValue(), b.GetValue());
    }

    public static float Distance(Color32 a, Color32 b)
    {
        float maxDistance = 255 * 255 * 3; // squared
        float distance = Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2);

        return distance / maxDistance;
    }

    public static float Match(Color32 a, Color32 b)
    {
        return 1 - Mathf.Clamp(Distance(a, b), 0f, 1f);
    }
    public static float Match(Color32 a, Color32 b, float range = 0.4f)
    {
        float distance = Distance(a, b);

        return 1f - Mathf.Clamp(distance, 0f, range) / range;
    }

    public static Gradient CreateColorGradient(Color a, Color b)
    {
        Gradient colorGradient = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = a;
        gak[0].alpha = a.a;
        gck[0].time = gak[0].time = 0f;
        gck[1].color = b;
        gak[1].alpha = b.a;
        gck[1].time = gak[0].time = 1f;
        colorGradient.SetKeys(gck, gak);
        colorGradient.SetKeys(gck, gak);
        return colorGradient;
    }
}
