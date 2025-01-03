using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selecting : MonoBehaviour
{
    private InputAction _selectAction;
    private InputAction _deselectAction;
    
    private bool _inputEnabled = true;
    private bool _isCreating = false;
    
    public Transform creationPoint;
    private GameObject _shape;
    private Vector3 _initPoint;
    private ColliderContainer _collider;
    
    public List<Selectable> selected = new();
    
    private void OnEnable()
    {
        _selectAction = Input.Instance.User.Select;
        _selectAction.Enable();
        _selectAction.performed += InitCreateShape;
        _selectAction.canceled += FinishCreateShape;
        
        _deselectAction = Input.Instance.User.Deselect;
        _deselectAction.Enable();
        _deselectAction.performed += ClearSelection;
    }

    private void OnDisable()
    {
        _selectAction.Disable();
        _deselectAction.Disable();
    }

    private void ClearSelection(InputAction.CallbackContext ctx)
    {
        selected.Clear();
    }
    
    private void FixedUpdate()
    {
        if (!_inputEnabled || !_isCreating) return;
        float button = _selectAction.ReadValue<float>();
        if (button == 0f) return;

        var size = Vector3.Magnitude(creationPoint.transform.position - _initPoint);
        _shape.transform.localScale = new Vector3(size, size, size);
    }
    
    private void InitCreateShape(InputAction.CallbackContext ctx)
    {
        _isCreating = true;
        _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _collider =_shape.AddComponent<ColliderContainer>();
        Rigidbody rb = _shape.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Material material = new(Shader.Find("Standard"));
        material.color = Color.blue;
        _shape.GetComponent<MeshRenderer>().material = material;
        _shape.transform.position = creationPoint.transform.position;
        _shape.transform.localScale = Vector3.zero;
        _initPoint = creationPoint.transform.position;
    }

    private void FinishCreateShape(InputAction.CallbackContext ctx)
    {
        foreach (var go in _collider.GetColliders())
        {
            Selectable selectable = go.GetComponent<Selectable>();
            if (selectable != null) selected.Add(selectable);
        }

        _isCreating = false;
        Destroy(_shape);
    }
    
    private class ColliderContainer : MonoBehaviour {
        private HashSet<GameObject> _colliders = new();

        public HashSet<GameObject> GetColliders () { return _colliders; }
 
        private void OnTriggerEnter (Collider other) {
            Debug.Log("enter");
            _colliders.Add(other.gameObject);
        }
 
        private void OnTriggerExit (Collider other) {
            _colliders.Remove(other.gameObject);
        }
    }
}
