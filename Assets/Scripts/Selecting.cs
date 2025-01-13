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
    public bool alignPosition { private get; set; }
    
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

    private void ClearSelection(InputAction.CallbackContext ctx)
    {
        selected.ToList().ForEach(selectedObject => selectedObject.Deselect());
        selected.Clear();
    }
    
    private void FixedUpdate()
    {
        float selectionButton = _selectAction.ReadValue<float>();
        if (selectionButton != 0f)
        {
            _shape.transform.position = (creationPoint.transform.position + _initPoint) / 2;
            _shape.transform.localScale = new Vector3(
                Mathf.Abs(creationPoint.transform.position.x - _initPoint.x),
                Mathf.Abs(creationPoint.transform.position.y - _initPoint.y),
                Mathf.Abs(creationPoint.transform.position.z - _initPoint.z)
            );
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
                    RotateSelection(creationPoint.transform.rotation * Quaternion.Inverse(_prevRotation));
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
        _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _collider =_shape.AddComponent<ColliderContainer>();
        Rigidbody rb = _shape.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        
        Material material = new(Shader.Find("Custom/TransparentShader"));
        
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
            if (selectable != null)
            {
                selected.Add(selectable);
                selectable.Select();
            }
        }

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

    public void Alignment(int alignment)
    {
        if (alignPosition)
            AlignPosition(alignment);
        else 
            AlignRotation(alignment);
    }

    public void AlignPosition(int alignment)
    {
        var list = selected.ToList();
        var positions = list.Select(selectable => selectable.gameObject.transform.position);
        switch ((Alignments)alignment)
        {
            case Alignments.Left:
                float lowestX = positions.Min(position => position.x);
                list.ForEach(selectedObject => selectedObject.SetXPosition(lowestX));
                break;
            case Alignments.Right:
                float highestX = positions.Max(position => position.x);
                list.ForEach(selectedObject => selectedObject.SetXPosition(highestX));
                break;
            case Alignments.Bottom:
                float lowestY = positions.Min(position => position.y);
                list.ForEach(selectedObject => selectedObject.SetYPosition(lowestY));
                break;
            case Alignments.Top:
                float highestY = positions.Max(position => position.y);
                list.ForEach(selectedObject => selectedObject.SetYPosition(highestY));
                break;
            case Alignments.Front:
                float lowestZ = positions.Min(position => position.z);
                list.ForEach(selectedObject => selectedObject.SetZPosition(lowestZ));
                break;
            case Alignments.Back:
                float highestZ = positions.Max(position => position.z);
                list.ForEach(selectedObject => selectedObject.SetZPosition(highestZ));
                break;
        }
    }
    
    public void AlignRotation(int alignment)
    {
        var list = selected.ToList();
        switch ((Alignments)alignment)
        {
            case Alignments.Top:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.identity));
                break;
            case Alignments.Bottom:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.Euler(180f, 0f, 0f)));
                break;
            case Alignments.Left:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.Euler(0f, 0f, 90f)));
                break;
            case Alignments.Right:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.Euler(0f, 0f, -90f)));
                break;
            case Alignments.Front:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.Euler(-90f, 0f, 0f)));
                break;
            case Alignments.Back:
                list.ForEach(selectedObject => selectedObject.SetRotation(Quaternion.Euler(90f, 0f, 0f)));
                break;
        }
    }
    
    private class ColliderContainer : MonoBehaviour {
        private HashSet<GameObject> _colliders = new();

        public HashSet<GameObject> GetColliders () { return _colliders; }
 
        private void OnTriggerEnter (Collider other) {
            _colliders.Add(other.gameObject);
        }
 
        private void OnTriggerExit (Collider other) {
            _colliders.Remove(other.gameObject);
        }
    }
}
