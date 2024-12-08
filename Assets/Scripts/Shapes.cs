using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shapes : MonoBehaviour
{
    private InputAction _interactAction;
    private bool _inputEnabled = true;
    private bool _isCreating = false;
    
    public Transform creationPoint; 
    private GameObject _shape;
    private Vector3 _initPoint;
    public Color ShapeColor { get; set; } = Color.red;
    public ShapeType SelectedShapeType { get; set; } = ShapeType.Cube;

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
    }

    private void InitCreateShape(InputAction.CallbackContext ctx)
    {
        _isCreating = true;

        switch (SelectedShapeType)
        {
            case ShapeType.Line:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
            case ShapeType.Plane:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Plane);
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
                _shape = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;
        }
        
        // TODO: adjust rotation and init size
        
        Material material = new(Shader.Find("Standard"));
        material.color = ShapeColor;
        _shape.GetComponent<MeshRenderer>().material = material;
        _shape.transform.position = creationPoint.transform.position;
        _initPoint = creationPoint.transform.position;
    }
    
    private void FinishCreateShape(InputAction.CallbackContext ctx)
    {
        _isCreating = false;
    }
}
