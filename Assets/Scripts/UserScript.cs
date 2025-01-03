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
    
    private Mode _currentMode = Mode.Shape;
    private Drawing _drawing;
    private Erasing _erasing;
    private Shapes _shapes;

    public enum Mode
    {
        Drawing,
        Erasing,
        Shape
    }

    // Input
    private Input _inputs;
    private InputAction _moveControllerAction;
    private InputAction _lookMouseAction;
    private InputAction _menuAction;
    private InputAction _shapeMenuAction;

    public float mouseSensitivity = 20f;
    private float _rotationX;
    private float _rotationY;

    // General
    private OVRCameraRig _cameraRig;

    private void Awake()
    {
        _inputs = Input.Instance;
    }

    private void OnEnable()
    {
        _moveControllerAction = _inputs.User.Move;
        _moveControllerAction.Enable();

        _lookMouseAction = _inputs.User.Look;
        _lookMouseAction.Enable();

        _menuAction = _inputs.User.Menu;
        _menuAction.Enable();
        _menuAction.performed += OpenMenu;
        _menuAction.canceled += CloseMenu;
        
        _shapeMenuAction = _inputs.User.ShapeMenu;
        _shapeMenuAction.Enable();
        _shapeMenuAction.performed += OpenShapeMenu;
        _shapeMenuAction.canceled += CloseShapeMenu;
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

        if (OVRManager.OVRManagerinitialized)
        {
            // Adjust height of camera
            var localPos = _cameraRig.transform.localPosition;
            localPos.y = OVRManager.profile.eyeHeight - 1;
            _cameraRig.transform.localPosition = localPos;
        }
        else
        {
            rightController.Translate(0.2f, 0, .4f);
            _debug = true;
        }

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        drawingCanvas.SetActive(false);
        shapeCanvas.SetActive(false);
        rayInteractor.SetActive(false);
        _drawing.EnableInputs();
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
    }

    private void EnableMode()
    {
        switch (_currentMode)
        {
            case Mode.Drawing: _drawing.EnableInputs(); break;
            case Mode.Erasing: _erasing.EnableInputs(); break;
            case Mode.Shape: _shapes.EnableInputs(); break;
        }
    }
    
    private void DisableMode()
    {
        switch (_currentMode)
        {
            case Mode.Drawing: _drawing.DisableInputs(); break;
            case Mode.Erasing: _erasing.DisableInputs(); break;
            case Mode.Shape: _shapes.DisableInputs(); break;
        }
    }

    public void ChangeMode(Mode mode)
    {
        _currentMode = mode;
    }

    private void OpenMenu(InputAction.CallbackContext ctx)
    {
        DisableMode();
        rayInteractor.SetActive(true);
        shapeCanvas.SetActive(false);
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
        shapeCanvas.SetActive(true);
    }
    
    private void CloseShapeMenu(InputAction.CallbackContext ctx)
    {
        shapeCanvas.SetActive(false);
        rayInteractor.SetActive(false);
        EnableMode();
    }
}