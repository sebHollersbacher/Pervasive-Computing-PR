using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UserScript : MonoBehaviour
{
    private bool _debug;
    public Transform rightController;
    public GameObject rayInteractor;
    
    public GameObject drawingCanvas;
    public GameObject shapeCanvas;
    public GameObject selectionCanvas;
    public GameObject alignmentObject;
    
    private Mode _currentMode = Mode.Shape;
    private Mode _prevMode = Mode.Shape;
    private Drawing _drawing;
    private Erasing _erasing;
    private Shapes _shapes;
    private Selecting _selection;
    private Shaping _shaping;

    public enum Mode
    {
        Drawing,
        Erasing,
        Shape,
        Selection,
        Shaping
    }

    // Input
    private InputAction _moveControllerAction;
    private InputAction _lookMouseAction;
    private InputAction _menuAction;
    private InputAction _shapeMenuAction;
    private InputAction _selectionMenuAction;
    private InputAction _shapingAction;

    public float mouseSensitivity = 20f;
    private float _rotationX;
    private float _rotationY;

    private GameObject x_line;
    private GameObject y_line;
    private GameObject z_line;

    // General
    private OVRCameraRig _cameraRig;

    private void OnEnable()
    {
        _moveControllerAction = Input.Instance.User.Move;
        _moveControllerAction.Enable();

        _lookMouseAction = Input.Instance.User.Look;
        _lookMouseAction.Enable();

        _menuAction = Input.Instance.User.Menu;
        _menuAction.Enable();
        _menuAction.performed += OpenMenu;
        _menuAction.canceled += CloseMenu;
        
        _shapeMenuAction = Input.Instance.User.ShapeMenu;
        _shapeMenuAction.Enable();
        _shapeMenuAction.performed += OpenShapeMenu;
        _shapeMenuAction.canceled += CloseShapeMenu;
        
        _selectionMenuAction = Input.Instance.User.SelectionMenu;
        _selectionMenuAction.Enable();
        _selectionMenuAction.performed += OpenSelectionMenu;
        _selectionMenuAction.canceled += CloseSelectionMenu;
        
        _shapingAction = Input.Instance.User.Shaping;
        _shapingAction.Enable();
        _shapingAction.performed += ChangeShaping;
    }
    
    private void ChangeShaping(InputAction.CallbackContext ctx)
    {
        DisableMode();
        if (_currentMode == Mode.Shaping)
        {
            _currentMode = _prevMode;
            _selection.isShaping = false;
        }
        else
        {
            _prevMode = _currentMode;
            _currentMode = Mode.Shaping;
            _selection.isShaping = true;
        }
        EnableMode();
    }

    private void OnDisable()
    {
        _moveControllerAction.Disable();
        _lookMouseAction.Disable();
        _menuAction.Disable();
        _shapeMenuAction.Disable();
    }

    private void Start()
    {
        _cameraRig = GetComponentInChildren<OVRCameraRig>();
        _drawing = GetComponentInChildren<Drawing>();
        _erasing = GetComponentInChildren<Erasing>();
        _shapes = GetComponentInChildren<Shapes>();
        _selection = GetComponentInChildren<Selecting>();
        _shaping = GetComponentInChildren<Shaping>();

        if (OVRManager.OVRManagerinitialized)
        {
            // Adjust height of camera
            var localPos = _cameraRig.transform.localPosition;
            localPos.y = OVRManager.profile.eyeHeight - .5f;
            _cameraRig.transform.localPosition = localPos;
        }
        else
        {
            rightController.Translate(0.2f, 0, .4f);
            var localPos = _cameraRig.transform.localPosition;
            localPos.y += 1;
            _cameraRig.transform.localPosition = localPos;
            _debug = true;
        }

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        drawingCanvas.SetActive(false);
        shapeCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        alignmentObject.SetActive(false);
        rayInteractor.SetActive(false);
        
        Material material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(1f, 0f, 0f, .2f);
        x_line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        x_line.GetComponent<MeshRenderer>().material = material;
        x_line.transform.position = rightController.position;
        x_line.transform.localScale = new Vector3(.1f, .1f, .1f);
        
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 1f, 0f, .2f);
        y_line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        y_line.GetComponent<MeshRenderer>().material = material;
        y_line.transform.position = rightController.position;
        y_line.transform.localScale = new Vector3(.1f, .1f, .1f);
            
        material = new(Shader.Find("Custom/TransparentShader"));
        material.color = new Color(0f, 0f, 1f, .2f);
        z_line = GameObject.CreatePrimitive(PrimitiveType.Cube);
        z_line.GetComponent<MeshRenderer>().material = material;
        z_line.transform.position = rightController.position;
        z_line.transform.localScale = new Vector3(.1f, .1f, .1f);
        
        DisableMode();
        EnableMode();
    }

    private void Update()
    {
        #region Move Controller

        if (_debug)
        {
            Vector3 input = _moveControllerAction.ReadValue<Vector3>();
            rightController.Translate(input * Time.deltaTime);
        }

        #endregion

        #region Look around

        if (_debug)
        {
            Vector3 input = _lookMouseAction.ReadValue<Vector2>();
            _rotationX += -input.y * mouseSensitivity * Time.deltaTime;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
            _rotationY += input.x * mouseSensitivity * Time.deltaTime;
            _cameraRig.transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        }

        #endregion
        
        Vector3 pos = rightController.position;
        pos.x -= 0.3f;
        x_line.transform.position = pos;
        
        pos = rightController.position;
        pos.y -= 0.3f;
        y_line.transform.position = pos;
        
        pos = rightController.position;
        pos.z -= 0.3f;
        z_line.transform.position = pos;
    }
    
    private void FixedUpdate()
    {
        if (alignmentObject.activeSelf)
        {
            alignmentObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }

    private void EnableMode()
    {
        switch (_currentMode)
        {
            case Mode.Drawing: _drawing.EnableInputs(); break;
            case Mode.Erasing: _erasing.EnableInputs(); break;
            case Mode.Shape: _shapes.EnableInputs(); break;
            case Mode.Selection: _selection.EnableInputs(); break;
            case Mode.Shaping: _shaping.EnableInputs(); break;
        }
    }
    
    private void DisableMode()
    {
        _drawing.DisableInputs();
        _erasing.DisableInputs();
        _shapes.DisableInputs();
        _selection.DisableInputs();
        _shaping.DisableInputs();
    }

    public void ChangeMode(Mode mode)
    {
        _currentMode = mode;
        DisableMode();
        EnableMode();
    }

    private void OpenMenu(InputAction.CallbackContext ctx)
    {
        DisableMode();
        rayInteractor.SetActive(true);
        shapeCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        drawingCanvas.SetActive(true);
    }

    private void CloseMenu(InputAction.CallbackContext ctx)
    {
        drawingCanvas.SetActive(false);
        rayInteractor.SetActive(false);
        EnableMode();
    }

    private void OpenShapeMenu(InputAction.CallbackContext ctx)
    {
        DisableMode();
        rayInteractor.SetActive(true);
        drawingCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        shapeCanvas.SetActive(true);
    }
    
    private void CloseShapeMenu(InputAction.CallbackContext ctx)
    {
        shapeCanvas.SetActive(false);
        rayInteractor.SetActive(false);
        EnableMode();
    }
    
    private void OpenSelectionMenu(InputAction.CallbackContext ctx)
    {
        DisableMode();
        rayInteractor.SetActive(true);
        drawingCanvas.SetActive(false);
        shapeCanvas.SetActive(false);
        selectionCanvas.SetActive(true);
    }
    
    private void CloseSelectionMenu(InputAction.CallbackContext ctx)
    {
        selectionCanvas.SetActive(false);
        alignmentObject.SetActive(false);
        rayInteractor.SetActive(false);
        EnableMode();
    }

    public void OpenAlignMenu(bool position)
    {
        selectionCanvas.SetActive(false);
        alignmentObject.SetActive(true);
        _selection.alignPosition = position;
    }
}