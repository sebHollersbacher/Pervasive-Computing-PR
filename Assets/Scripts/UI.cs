using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Drawing drawingScript;
    public Erasing erasingScript;
    
    public Button drawingButton;
    public Button eraseButton;

    private void Start()
    {
        drawingButton.interactable  = false;
        erasingScript.DisableInputs();
        drawingScript.EnableInputs();
    }

    public void DrawingButton()
    {
        drawingButton.interactable  = false;
        eraseButton.interactable  = true;
        
        erasingScript.DisableInputs();
        drawingScript.EnableInputs();
    }

    public void ErasingButton()
    {
        drawingButton.interactable  = true;
        eraseButton.interactable  = false;
        
        drawingScript.DisableInputs();
        erasingScript.EnableInputs();
    }

    public void SizeChanged(float value)
    {
        drawingScript.ChangeRadius(0.001f + value * 0.03f);
    }

    public void ColorChanged(Color value)
    {
        drawingScript.ChangeLineColor(value);
    }
}