using UnityEngine;
using Random = System.Random;

[System.Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int _min, int _max)
    {
        if ( _min > _max)
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

    public int Width() { return max - min; }

    public float Match(IntRange target)
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
    public float Match(int a, int b)
    {
        if (Width() != 0)
        {
            return 1f - Mathf.Abs(a - b) / Width();
        }
        else
        {
            if (a == b) { return 0f; }
            else { return 1f; }
        }
    }

    public int GetValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
    public int GetValue(Random rand)
    {
        return rand.Next(min, max);
    }

    public static IntRange Combine(IntRange a, IntRange b)
    {
        // all the code to combine them is in Manager
        IntRangeManager temp = new IntRangeManager(
            Mathf.Min(a.min, b.min), Mathf.Max(a.max, b.max), 
            Mathf.Min(a.Width(), b.Width()), Mathf.Max(a.Width(), b.Width())
            );
        return temp.GetIntRange();
    }
}

[System.Serializable]
public struct IntRangeManager
{
    public int min;
    public int max;
    public int minW;
    public int maxW;

    public IntRangeManager(int _min, int _max, int _minW, int _maxW)
    {
        min = _min;
        max = _max;
        minW = _minW;
        maxW = _maxW;
    }

    public IntRange GetIntRange()
    {
        // get a int range subject to width constraints
        int width = UnityEngine.Random.Range(minW, maxW);
        int start = UnityEngine.Random.Range(min, max - width);
        return new IntRange(start, start + width);
    }
}
