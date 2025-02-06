using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private UserScript _userScript;
    public Drawing drawingScript;
    public Erasing erasingScript;
    public Shapes shapeScript;
    public Selecting selectScript;
    public Shaping shapingScript;
    
    public GameObject drawingCanvas;
    public GameObject shapeCanvas;
    public GameObject selectionCanvas;
    
    private Button _selectedButton;

    public Transform coordinateSystemTransform;
    private GameObject _xLine;
    private GameObject _yLine;
    private GameObject _zLine;
    private bool _showLines;
    
    private void Start()
    {
        _userScript = GetComponentInChildren<UserScript>();
        ChangeSelectedButton(drawingCanvas.GetComponentsInChildren<Button>().Select(button => button).First(button => button.name == "DrawingButton"));
        
        Material material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(1f, 0f, 0f, .2f);
        _xLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _xLine.GetComponent<MeshRenderer>().material = material;
        _xLine.transform.localScale = Vector3.zero;
        
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 1f, 0f, .2f);
        _yLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _yLine.GetComponent<MeshRenderer>().material = material;
        _yLine.transform.localScale = Vector3.zero;
            
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 0f, 1f, .2f);
        _zLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _zLine.GetComponent<MeshRenderer>().material = material;
        _zLine.transform.localScale = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!_showLines) return;
        
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

    public void ChangeMode(Button button)
    {
        switch (button.name)
        {
            case "DrawingButton":
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Drawing;
                break;
            case "ErasingButton":
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Erasing;
                break;
            case "ShapesButton":
                _userScript.OpenShapeMenu();
                break;
            case "SelectionButton":
                _userScript.OpenSelectionMenu();
                break;
            case "ManipulationButton":
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Shaping;
                break;
        }
    }

    public void ChangeSelectedButton(Button newButton)
    {
        if (_selectedButton != null)
        {
            _selectedButton.interactable = true;
        }
        
        _selectedButton = newButton;
        
        if (_selectedButton != null)
        {
            _selectedButton.interactable = false;
        }
    }

    public void ToggleCoordinateMarkers(Button button)
    {
        _showLines = !_showLines;
        if (!_showLines)
        {
            _xLine.transform.localScale = Vector3.zero;
            _yLine.transform.localScale = Vector3.zero;
            _zLine.transform.localScale = Vector3.zero;
        }
        button.targetGraphic.color = _showLines ? new Color(0f, .7f, 1f, 1f) : Color.white;
    }
    
    public void HandleSelectionButton(Button button)
    {
        switch (button.name)
        {
            case "DeleteButton": 
                selectScript.DeleteSelection(); 
                break;
            case "MoveButton":
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Selection;
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Move);
                break;
            case "RotateButton": 
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Selection;
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Rotate); 
                break;
            case "ScaleButton": 
                ChangeSelectedButton(button);
                _userScript.CurrentMode = UserScript.Mode.Selection;
                selectScript.ChangeSelectionMode(Selecting.SelectionMode.Scale); 
                break;
            case "AlignPositionButton": _userScript.OpenAlignMenu(true); break;
            case "AlignRotationButton": _userScript.OpenAlignMenu(false); break;
        }
    } 

    public void ChangeShapeButton(Button button)
    {
        ChangeSelectedButton(button);
        _userScript.CurrentMode = UserScript.Mode.Shape;

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
        shapingScript.ChangeRadius(0.001f + value * 0.03f);
        shapeScript.Radius = 0.001f + value * 0.03f;
    }

    public void ColorChanged(Color value)
    {
        drawingScript.ChangeLineColor(value);
        shapeScript.ShapeColor = value;
    }
}