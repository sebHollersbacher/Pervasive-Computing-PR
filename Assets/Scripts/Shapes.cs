using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shapes : MonoBehaviour
{
    private InputAction _interactAction;
    private bool _inputEnabled = false;
    private bool _isCreating = false;

    public Transform creationPoint;
    private GameObject _shape;
    private Vector3 _initPoint;
    public Color ShapeColor { get; set; } = Color.red;
    public ShapeType SelectedShapeType { get; set; } = ShapeType.Cube;
    public float Radius { get; set; } = 0.03f;

    public enum ShapeType
    {
        Line,
        Plane,
        Cube,
        Sphere,
        Cylinder,
        Pyramid
    }

    #region Inputs

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();

        _interactAction.performed += InitCreateShape;
        _interactAction.canceled += FinishCreateShape;
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    public void DisableInputs()
    {
        _inputEnabled = false;
        _interactAction.Disable();
    }

    public void EnableInputs()
    {
        _interactAction.Enable();
        _inputEnabled = true;
    }

    #endregion

    private void FixedUpdate()
    {
        if (!_inputEnabled || !_isCreating) return;
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;

        var size = Vector3.Magnitude(creationPoint.transform.position - _initPoint);
        _shape.transform.localScale = new Vector3(size, size, size);

        Vector3 newPoint = creationPoint.transform.position;
        _shape.transform.rotation = Quaternion.LookRotation(newPoint - _initPoint) * Quaternion.Euler(90, 0, 0);
        if (SelectedShapeType == ShapeType.Plane)
        {
            _shape.transform.localScale = new Vector3(0.001f, size, size);
        }
        if (SelectedShapeType == ShapeType.Line)
        {
            _shape.transform.localScale = new Vector3(2*Radius, size/2, 2*Radius);
            _shape.transform.position = (_initPoint + creationPoint.transform.position) * 0.5f;
        }
    }

    private void InitCreateShape(InputAction.CallbackContext ctx)
    {
        _isCreating = true;

        switch (SelectedShapeType)
        {
            case ShapeType.Line:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                _shape.name = "Line";
                break;
            case ShapeType.Plane:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _shape.name = "Plane";
                break;
            case ShapeType.Cube:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case ShapeType.Sphere:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case ShapeType.Cylinder:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case ShapeType.Pyramid:
                _shape = CreatePyramid();
                break;
        }

        Material material = new(Shader.Find("Standard"));
        material.color = ShapeColor;
        _shape.GetComponent<MeshRenderer>().material = material;
        _shape.transform.position = creationPoint.transform.position;
        _shape.transform.localScale = Vector3.zero;
        _initPoint = creationPoint.transform.position;
    }

    private void FinishCreateShape(InputAction.CallbackContext ctx)
    {
        _shape.GetComponent<Collider>().isTrigger = true;
        _shape.AddComponent<Selectable>();
        _isCreating = false;
    }
    
    private GameObject CreatePyramid()
    {
        GameObject pyramid = new("Pyramid");
        Mesh mesh = new Mesh();
        pyramid.AddComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices =
        {
            new(0, 1, 0),
            new(-0.5f, 0, -0.5f),
            new(0.5f, 0, -0.5f),
            new(0.5f, 0, 0.5f),
            new(-0.5f, 0, 0.5f)
        };

        int[] triangles =
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3,
            0, 1, 4,
            1, 2, 3,
            1, 3, 4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        pyramid.AddComponent<MeshRenderer>();
        var meshCollider = pyramid.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        
        return pyramid;
    }
}