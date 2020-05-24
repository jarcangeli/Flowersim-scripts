using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMatch : MonoBehaviour
{

    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public Color color3 = Color.magenta;

    // Start is called before the first frame update
    void Start()
    {
        FloatRange range1 = new FloatRange(0f, 1f);

        FloatRange range2 = new FloatRange(0.8f, 1.2f);

        FloatRange range3 = new FloatRange(1.1f, 1.2f);

        Debug.Log(range1.Match(range2));
        Debug.Log(range1.Match(range3));

        Debug.Log(range2.Match(range1));
        Debug.Log(range2.Match(range3));

        Debug.Log(range3.Match(range1));
        Debug.Log(range3.Match(range2));

        Debug.Log("Check colors:");


        Debug.Log(ColorRange.Match(color1, color2));
        Debug.Log(ColorRange.Match(color1, color3));
        Debug.Log(ColorRange.Match(color3, color2));
    }
}
