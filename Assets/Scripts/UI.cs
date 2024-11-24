using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Drawing drawingScript;

    public void DrawingButton()
    {
    }

    public void ErasingButton()
    {
    }

    public void SizeChanged(float value)
    {
        drawingScript.radius = (value + 1) * 0.01f;
    }

    public void ColorChanged(Color value)
    {
        drawingScript.material.color = value;
    }
}