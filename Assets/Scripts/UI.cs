using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private UserScript _userScript;
    public Drawing drawingScript;
    public Erasing erasingScript;
    public Shapes shapeScript;
    public Selecting selectScript;
    
    public GameObject drawingCanvas;
    public GameObject shapeCanvas;
    public GameObject selectionCanvas;
    
    private Button _drawingButton;
    private Button _eraseButton;
    
    private Button _lineButton;
    private Button _planeButton;
    private Button _cubeButton;
    private Button _sphereButton;
    private Button _cylinderButton;
    private Button _pyramidButton;
    
    private Button _deleteButton;
    private Button _moveButton;
    private Button _rotateButton;
    private Button _scaleButton;
    private Button _alignPosButton;
    private Button _alignRotButton;

    public Transform coordinateSystemTransform;
    private GameObject _xLine;
    private GameObject _yLine;
    private GameObject _zLine;

    private void Start()
    {
        _userScript = GetComponentInChildren<UserScript>();
        var buttons = drawingCanvas.GetComponentsInChildren<Button>();
        AssignButtons(buttons);
        buttons = shapeCanvas.GetComponentsInChildren<Button>();
        AssignButtons(buttons);
        buttons = selectionCanvas.GetComponentsInChildren<Button>();
        AssignButtons(buttons);
        
        DisableAll();
        
        Material material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(1f, 0f, 0f, .2f);
        _xLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _xLine.GetComponent<MeshRenderer>().material = material;
        
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 1f, 0f, .2f);
        _yLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _yLine.GetComponent<MeshRenderer>().material = material;
            
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 0f, 1f, .2f);
        _zLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _zLine.GetComponent<MeshRenderer>().material = material;
    }

    private void FixedUpdate()
    {
        Vector3 refPoint = coordinateSystemTransform.position;
        refPoint.x = refPoint.x/2 - 1.5f;
        _xLine.transform.position = refPoint;
        _xLine.transform.localScale = new Vector3((refPoint.x+3)*2, .01f, .01f);
        
        refPoint = coordinateSystemTransform.position;
        refPoint.y = refPoint.y/2 - 1.5f;
        _yLine.transform.position = refPoint;
        _yLine.transform.localScale = new Vector3(.01f, (refPoint.y+3)*2, .01f);
        
        refPoint = coordinateSystemTransform.position;
        refPoint.z = refPoint.z/2 - 1.5f;
        _zLine.transform.position = refPoint;
        _zLine.transform.localScale = new Vector3(.01f, .01f, (refPoint.z+3)*2);
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
    
    public void HandleSelectionButton(Button button)
    {
        DisableAll();

        switch (button.name)
        {
            case "DeleteButton": selectScript.DeleteSelection(); break;
            case "MoveButton":
                button.interactable = false;
                _userScript.ChangeMode(UserScript.Mode.Selection);
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Move);
                break;
            case "RotateButton": 
                button.interactable = false;
                _userScript.ChangeMode(UserScript.Mode.Selection);
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Rotate); 
                break;
            case "ScaleButton": 
                button.interactable = false;
                _userScript.ChangeMode(UserScript.Mode.Selection);
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Scale); 
                break;
            case "AlignPositionButton": _userScript.OpenAlignMenu(true); break;
            case "AlignRotationButton": _userScript.OpenAlignMenu(false); break;
        }
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
        shapeScript.Radius = 0.001f + value * 0.03f;
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
        
        _moveButton.interactable = true;
        _rotateButton.interactable = true;
        _scaleButton.interactable = true;
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
                case "CylinderButton": _cylinderButton = button; break;
                case "PyramidButton": _pyramidButton = button; break;
                
                case "MoveButton": _moveButton = button; break;
                case "RotateButton": _rotateButton = button; break;
                case "ScaleButton": _scaleButton = button; break;
            }
        }
    }
}