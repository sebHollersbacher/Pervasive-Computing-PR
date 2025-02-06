using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shaping : MonoBehaviour
{
    private InputAction _interactAction;
    private InputAction _createAction;
    
    public GameObject shaper;
    private ColliderContainer _collider;
    
    private bool isActive;
    
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = shaper.transform.position;
        _collider = shaper.AddComponent<ColliderContainer>();
        Rigidbody rb = shaper.AddComponent<Rigidbody>();
        rb.isKinematic = true;
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
        SceneManager.Instance.GetShapeables().ForEach(shapeable =>
        {
            if(shapeable.SelectedVertices.Count > 0)
                shapeable.CreateEdges();
        });
        SceneManager.Instance.GetShapeables().ForEach(shapeable =>
        {
            if(shapeable.SelectedEdges.Count > 0)
                shapeable.CreateVertices();
        });
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
    
    public void ChangeRadius(float radius)
    {
        shaper.transform.localScale = new Vector3(2*radius, 2*radius, 2*radius);
    }
    
    private class ColliderContainer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != null)
            {
                var vertex = other.gameObject.GetComponent<Vertex>();
                if (vertex != null) vertex.Select();
                
                var edge = other.gameObject.GetComponent<Edge>();
                if (edge != null) edge.Select();
            }
        }
    }
}
