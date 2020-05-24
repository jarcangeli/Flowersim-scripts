using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTest : MonoBehaviour
{
    public Color color1, color2;
    public Gradient gradient;

    private void Update()
    {
        if (color1 != null && color2 != null)
        {
            gradient = ColorRange.CreateColorGradient(color1, color2);
        }
    }
}
