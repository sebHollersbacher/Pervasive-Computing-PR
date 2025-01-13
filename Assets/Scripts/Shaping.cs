using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shaping : MonoBehaviour
{
    private InputAction _interactAction;
    
    public GameObject shaper;
    
    private ShapingCollider _shapeCollider;

    private bool isActive;
    
    private Vector3 startPosition;

    private void Awake()
    {
        _shapeCollider = shaper.GetComponent<ShapingCollider>();
    }

    private void Start()
    {
        startPosition = shaper.transform.position;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        var difference = shaper.transform.position - startPosition;
        startPosition = shaper.transform.position;
        
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;
        
        _shapeCollider.shapeables.ForEach(s => s.Move(difference));
    }

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }
    
    public void DisableInputs()
    {
        shaper.SetActive(false);
        isActive = false;
        _interactAction.Disable();
    }

    public void EnableInputs()
    {
        shaper.SetActive(true);
        isActive = true;
        _interactAction.Enable();
    }
}
