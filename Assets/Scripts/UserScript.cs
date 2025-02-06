using UnityEngine;
using UnityEngine.InputSystem;

public class UserScript : MonoBehaviour
{
    private bool _debug;
    public Transform rightController;
    public GameObject rayInteractor;
    
    public GameObject drawingCanvas;
    public GameObject shapeCanvas;
    public GameObject selectionCanvas;
    public GameObject alignmentObject;

    private Mode _currentMode = Mode.Drawing;
    public Mode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode != value)
            {
                _currentMode = value;
                DisableMode();
            }
        }
    }

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

    public float mouseSensitivity = 20f;
    private float _rotationX;
    private float _rotationY;

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
        _menuAction.canceled += CloseMenus;
    }

    private void OnDisable()
    {
        _moveControllerAction.Disable();
        _lookMouseAction.Disable();
        _menuAction.Disable();
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
        switch (CurrentMode)
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

    private void OpenMenu(InputAction.CallbackContext ctx)
    {
        DisableMode();
        rayInteractor.SetActive(true);
        drawingCanvas.SetActive(true);
    }

    private void CloseMenus(InputAction.CallbackContext ctx)
    {
        drawingCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        shapeCanvas.SetActive(false);
        alignmentObject.SetActive(false);
        rayInteractor.SetActive(false);
        EnableMode();
    }

    public void OpenShapeMenu()
    {
        drawingCanvas.SetActive(false);
        shapeCanvas.SetActive(true);
    }
    
    public void OpenSelectionMenu()
    {
        drawingCanvas.SetActive(false);
        selectionCanvas.SetActive(true);
    }

    public void OpenAlignMenu(bool position)
    {
        selectionCanvas.SetActive(false);
        alignmentObject.SetActive(true);
        _selection.alignPosition = position;
    }
}