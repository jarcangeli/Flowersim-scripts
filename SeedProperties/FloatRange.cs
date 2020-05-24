using UnityEngine;
using Random = System.Random;

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public FloatRange(float _min, float _max)
    {
        if (_min > _max)
        {
            min = _max;
            max = _min;
        }
        else
        {
            min = _min;
            max = _max;
        }
    }

    public float Width() { return max - min; }

    public float Match( FloatRange target)
    {
        // calculate the % match of a float range to this one
        // overlap is the fraction of own rng that could be from target
        float overlap = target.Width() / Width();
        // partial overlap
        if (max > target.max && min > target.min)
        {
            overlap = (target.max - min) / Width();
        }
        else if (min < target.min && max < target.max)
        {
            overlap = (max - target.min) / Width();
        }
        return Mathf.Clamp(overlap, 0f, 1f);
    }

    public float Match(float a, float b)
    {
        if (Width() != 0)
        {
            return 1f - Mathf.Abs(a - b) / Width();
        }
        else
        {
            if (a == b) { return 1f; }
            else { return 0f; }
        }
    }

    public float GetValue()
    {
        // get a value within the range
        return UnityEngine.Random.value * (max - min) + min;
    }
    public float GetValue(Random rand)
    {
        // get a value within the range
        return (float)rand.NextDouble() * (max - min) + min;
    }
    public static FloatRange Combine(FloatRange a, FloatRange b)
    {
        // all the code to combine them is in Manager
        FloatRangeManager temp = new FloatRangeManager(
            Mathf.Min(a.min, b.min), Mathf.Max(a.max, b.max),
            Mathf.Min(a.Width(), b.Width()), Mathf.Max(a.Width(), b.Width())
            );
        return temp.GetFloatRange();
    }
}

[System.Serializable]
public struct FloatRangeManager
{
    public float min;
    public float max;
    public float minW;
    public float maxW;

    public FloatRangeManager(float _min, float _max, float _minW, float _maxW)
    {
        min = _min;
        max = _max;
        minW = _minW;
        maxW = _maxW;
    }

    public FloatRange GetFloatRange()
    {
        // get a float range subject to width constraints
        float width = UnityEngine.Random.value * (maxW - minW) + minW;
        float start = UnityEngine.Random.value * (max - min - width) + min;
        return new FloatRange(start, start+width);
    }
}
