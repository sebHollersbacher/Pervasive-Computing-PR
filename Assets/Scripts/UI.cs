using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private UserScript _userScript;
    
    public Drawing drawingScript;
    public Erasing erasingScript;
    public Shapes shapeScript;
    
    public GameObject drawingCanvas;
    public GameObject shapeCanvas;
    
    private Button _drawingButton;
    private Button _eraseButton;
    
    private Button _lineButton;
    private Button _planeButton;
    private Button _cubeButton;
    private Button _sphereButton;
    private Button _cylinderButton;
    private Button _pyramidButton;
    
    private void Start()
    {
        _userScript = GetComponentInChildren<UserScript>();
        
        var buttons = drawingCanvas.GetComponentsInChildren<Button>();
        AssignButtons(buttons);
        buttons = shapeCanvas.GetComponentsInChildren<Button>();
        AssignButtons(buttons);
        
        DisableAll();
        DrawingButton();
    }

    public void DrawingButton()
    {
        DisableAll();
        _drawingButton.interactable = false;
        _userScript.ChangeMode(UserScript.Mode.Drawing);
    }

    public void ErasingButton()
    {
        DisableAll();
        _eraseButton.interactable = false;
        _userScript.ChangeMode(UserScript.Mode.Erasing);
    }

    public void ChangeShapeButton(Button button)
    {
        DisableAll();
        button.interactable = false;
        _userScript.ChangeMode(UserScript.Mode.Shape);

        shapeScript.SelectedShapeType = button.name switch
        {
            "LineButton" => Shapes.ShapeType.Line,
            "PlaneButton" => Shapes.ShapeType.Plane,
            "CubeButton" => Shapes.ShapeType.Cube,
            "SphereButton" => Shapes.ShapeType.Sphere,
            "CylinderButton" => Shapes.ShapeType.Cylinder,
            "PyramidButton" => Shapes.ShapeType.Pyramid,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void SizeChanged(float value)
    {
        drawingScript.ChangeRadius(0.001f + value * 0.03f);
    }

    public void ColorChanged(Color value)
    {
        drawingScript.ChangeLineColor(value);
        shapeScript.ShapeColor = value;
    }

    private void DisableAll()
    {
        _drawingButton.interactable = true;
        _eraseButton.interactable = true;
        _lineButton.interactable = true;
        _planeButton.interactable = true;
        _cubeButton.interactable = true;
        _sphereButton.interactable = true;
        _cylinderButton.interactable = true;
        _pyramidButton.interactable = true;
        
        erasingScript.DisableInputs();
        drawingScript.DisableInputs();
    }
    
    private void AssignButtons(Button[] buttons)
    {
        foreach (var button in buttons)
        {
            switch (button.name)
            {
                case "DrawingButton": _drawingButton = button; break;
                case "ErasingButton": _eraseButton = button; break;
                
                case "LineButton": _lineButton = button; break;
                case "PlaneButton": _planeButton = button; break;
                case "CubeButton": _cubeButton = button; break;
                case "SphereButton": _sphereButton = button; break;
                case "CylinderButton":_cylinderButton = button; break;
                case "PyramidButton": _pyramidButton = button; break;
            }
        }
    }
}