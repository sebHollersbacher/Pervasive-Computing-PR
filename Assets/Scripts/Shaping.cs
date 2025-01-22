using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shaping : MonoBehaviour
{
    private InputAction _interactAction;
    private InputAction _createAction;
    
    public GameObject shaper;
    
    private bool isActive;
    
    private Vector3 startPosition;

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
        SceneManager.Instance.GetShapeables().ForEach(shapeable => shapeable.Move(difference));
    }

    private void CreateVertices(InputAction.CallbackContext obj)
    {
        SceneManager.Instance.GetShapeables().ForEach(shapeable => shapeable.CreateEdges());
    }

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();
        _createAction = Input.Instance.User.Create;
        _createAction.Enable();
        _createAction.performed += CreateVertices;
    }

    

    private void OnDisable()
    {
        _interactAction.Disable();
        _createAction.Disable();
    }
    
    public void DisableInputs()
    {
        SceneManager.Instance.GetShapeables().ForEach(shapeable => shapeable.SetActive(false));
        shaper.SetActive(false);
        isActive = false;
        _interactAction.Disable();
        _createAction.Disable();
    }

    public void EnableInputs()
    {
        SceneManager.Instance.GetShapeables().ForEach(shapeable => shapeable.SetActive(true));
        shaper.SetActive(true);
        isActive = true;
        _interactAction.Enable();
        _createAction.Enable();
    }
}
