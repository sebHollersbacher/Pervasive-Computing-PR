using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selecting : MonoBehaviour
{
    private InputAction _selectAction;
    private InputAction _deselectAction;
    private InputAction _interactAction;
    
    private bool _inputEnabled = true;
    private bool _isCreating = false;
    
    public Transform creationPoint;
    private GameObject _shape;
    private Vector3 _initPoint;
    private ColliderContainer _collider;
    
    public HashSet<Selectable> selected = new();

    private SelectionMode _mode = SelectionMode.Move;
    private Vector3 _prevPosition;
    private Quaternion _prevRotation;
    
    public enum SelectionMode
    {
        Move,
        Rotate,
        Scale
    }

    public enum Alignments
    {
        Top = 1,
        Left = 2,
        Front = 3,
        Back = 4,
        Right = 5,
        Bottom = 6
    }
    
    private void OnEnable()
    {
        _selectAction = Input.Instance.User.Select;
        _selectAction.Enable();
        _selectAction.performed += InitCreateShape;
        _selectAction.canceled += FinishCreateShape;
        
        _deselectAction = Input.Instance.User.Deselect;
        _deselectAction.Enable();
        _deselectAction.performed += ClearSelection;
        
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();
        _interactAction.performed += InitSelectionManipulation;
    }

    private void OnDisable()
    {
        _selectAction.Disable();
        _deselectAction.Disable();
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

    private void ClearSelection(InputAction.CallbackContext ctx)
    {
        selected.Clear();
    }
    
    private void FixedUpdate()
    {
        // if (!_isCreating) return;
        float selectionButton = _selectAction.ReadValue<float>();
        if (selectionButton != 0f)
        {
            var size = Vector3.Magnitude(creationPoint.transform.position - _initPoint);
            _shape.transform.localScale = new Vector3(size, size, size);
        }

        if (!_inputEnabled) return;
        float interactionButton = _interactAction.ReadValue<float>();
        if (interactionButton != 0f)
        {
            switch (_mode)
            {
                case SelectionMode.Move:
                    MoveSelection(creationPoint.transform.position - _prevPosition);
                    break;
                case SelectionMode.Rotate:
                    RotateSelection(_prevRotation * Quaternion.Inverse(creationPoint.transform.rotation));
                    break;
                case SelectionMode.Scale:
                    ScaleSelection(creationPoint.transform.position - _prevPosition);
                    break;
            }
        }
        _prevPosition = creationPoint.transform.position;
        _prevRotation = creationPoint.transform.rotation;
    }

    private void InitSelectionManipulation(InputAction.CallbackContext ctx)
    {
        _prevPosition = creationPoint.transform.position;
        _prevRotation = creationPoint.transform.rotation;
    }
    
    private void InitCreateShape(InputAction.CallbackContext ctx)
    {
        _isCreating = true;
        _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _collider =_shape.AddComponent<ColliderContainer>();
        Rigidbody rb = _shape.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Material material = new(Shader.Find("Standard"));   
        
        // Make material transparent (Not working for Quest yet)
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        material.color = new Color(0f, 0f, 1f, .5f);
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

    public void ChangeSelectionMode(SelectionMode mode)
    {
        _mode = mode;
    }

    public void DeleteSelection()
    {
        selected.ToList().ForEach(selectedObject => Destroy(selectedObject.gameObject));
        selected.Clear();
    }
    
    private void MoveSelection(Vector3 difference)
    {
        selected.ToList().ForEach(selectedObject => selectedObject.Move(difference));
    }
    
    private void RotateSelection(Quaternion difference)
    {
        selected.ToList().ForEach(selectedObject => selectedObject.Rotate(difference));
    }
    
    private void ScaleSelection(Vector3 difference)
    {
        selected.ToList().ForEach(selectedObject => selectedObject.Scale(difference));
    }

    public void AlignPosition(int alignment)
    {
        var list = selected.ToList();
        switch ((Alignments)alignment)
        {
            case Alignments.Top:
                break;
            case Alignments.Bottom:
                list.ForEach(sele => sele.SetXPosition(0));
                break;
            case Alignments.Left:
                list.ForEach(sele => sele.SetYPosition(0));
                break;
            case Alignments.Right:
                break;
            case Alignments.Front:
                break;
            case Alignments.Back:
                list.ForEach(sele => sele.SetZPosition(0));
                break;
        }
    }
    
    public void AlignRotation(int alignment)
    {
        var list = selected.ToList();
        switch ((Alignments)alignment)
        {
            case Alignments.Top:
                list.ForEach(sele => sele.SetRotation(new Quaternion(0f, 0f, 0f, 1f)));
                break;
            case Alignments.Bottom:
                list.ForEach(sele => sele.SetRotation(new Quaternion(0f, 0f, 90f, 1f)));
                break;
            case Alignments.Left:
                list.ForEach(sele => sele.SetRotation(new Quaternion(0f, 90f, 90f, 1f)));
                break;
            case Alignments.Right:
                list.ForEach(sele => sele.SetRotation(new Quaternion(45f, 0f, 0f, 1f)));
                break;
            case Alignments.Front:
                break;
            case Alignments.Back:
                break;
        }
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
