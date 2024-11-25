using UnityEngine;
using UnityEngine.InputSystem;

public class UserScript : MonoBehaviour
{
    private bool _debug;
    public Transform rightController;
    private Drawing _drawing;
    public GameObject rayInteractor;
    public GameObject canvas;

    // Input
    private Input _inputs;
    private InputAction _moveControllerAction;
    private InputAction _lookMouseAction;
    private InputAction _menuAction;

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

    private void OpenMenu(InputAction.CallbackContext ctx)
    {
        _drawing.DisableInputs();
        rayInteractor.SetActive(true);
        canvas.SetActive(true);
    }

    private void CloseMenu(InputAction.CallbackContext ctx)
    {
        canvas.SetActive(false);
        rayInteractor.SetActive(false);
        _drawing.EnableInputs();
    }
}