using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Drawing drawingScript;
    
    public Button drawingButton;
    public Button eraseButton;

    private void Start()
    {
        drawingButton.interactable  = false;
    }

    public void DrawingButton()
    {
        drawingButton.interactable  = false;
        eraseButton.interactable  = true;
    }

    public void ErasingButton()
    {
        drawingButton.interactable  = true;
        eraseButton.interactable  = false;
    }

    public void SizeChanged(float value)
    {
        drawingScript.radius = 0.001f + value * 0.03f;
    }

    public void ColorChanged(Color value)
    {
        drawingScript.lineColor = value;
    }
}