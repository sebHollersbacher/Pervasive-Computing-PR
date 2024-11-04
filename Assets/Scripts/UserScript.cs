using UnityEngine;
using UnityEngine.InputSystem;

public class UserScript : MonoBehaviour
{
    private bool DEBUG = false;
    public Transform rightController;

    private InputActions controllerMovement;
    private InputAction MoveControllerAction;
    private InputAction LookMouseAction;
    private InputAction InteractAction;

    private OVRCameraRig cameraRig;
    public GameObject cube;

    public float mouseSensitivity = 20f;
    private float rotationX = 0;
    private float rotationY = 0;

    private void Awake()
    {
        controllerMovement = new InputActions();
    }

    private void OnEnable()
    {
        MoveControllerAction = controllerMovement.User.Move;
        MoveControllerAction.Enable();

        LookMouseAction = controllerMovement.User.Look;
        LookMouseAction.Enable();


        InteractAction = controllerMovement.User.Interact;
        InteractAction.Enable();
        InteractAction.performed += Interact;
    }

    private void OnDisable()
    {
        MoveControllerAction.Disable();
        LookMouseAction.Disable();
        InteractAction.Disable();
    }

    void Start()
    {
        cameraRig = GetComponentInChildren<OVRCameraRig>();

        if (OVRManager.OVRManagerinitialized)
        {
            // Quest is used
        }
        else
        {
            Debug.Log("DEBUG!");
            DEBUG = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        #region Move Controller
        if (DEBUG)
        {
            Vector3 input = MoveControllerAction.ReadValue<Vector3>();
            rightController.Translate(input * Time.deltaTime);
        }
        #endregion

        #region Look around
        if (DEBUG)
        {
            Vector3 input = LookMouseAction.ReadValue<Vector2>();
            rotationX += -input.y * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            rotationY += input.x * mouseSensitivity * Time.deltaTime;
            cameraRig.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        #endregion
    }

    private void Interact(InputAction.CallbackContext callbackContext)
    {
        Instantiate(cube,rightController.position, rightController.rotation);
    }
}
