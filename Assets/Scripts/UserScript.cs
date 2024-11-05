using System.Collections.Generic;
using System.Linq;
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

    private List<MeshFilter> meshes = new List<MeshFilter>();
    private GameObject currentParent = null;
    public Material Material;
    
    private List<Vector3> linePoints = new List<Vector3>();
    private LineRenderer lineRenderer;
    private int pointIndex;

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
        InteractAction.performed += InitDraw;
        // InteractAction.canceled += EndDraw;
    }

    private void InitDraw(InputAction.CallbackContext context)
    {
        currentParent = new GameObject("Line");
        lineRenderer = currentParent.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = Material;
        lineRenderer.positionCount = 0;
        pointIndex = 0;
    }
    
    private void EndDraw(InputAction.CallbackContext context)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
            meshes[i].gameObject.SetActive(false);
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);
        currentParent.AddComponent<MeshFilter>().mesh = combinedMesh;
        currentParent.AddComponent<MeshRenderer>().material = Material;
        meshes.Clear();
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
            // Adjust height of camera
            Vector3 localPos = cameraRig.transform.localPosition;
            localPos.y = OVRManager.profile.eyeHeight - 1;
            cameraRig.transform.localPosition = localPos;
        }
        else
        {
            rightController.Translate(0.2f, 0, .4f);
            Debug.Log("DEBUG!");
            DEBUG = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void CreateObject()
    {
        float button = InteractAction.ReadValue<float>();
        if (button != 0f)
        {
            GameObject obj = Instantiate(cube, rightController.position, rightController.rotation,
                currentParent.transform);
            meshes.Add(obj.GetComponent<MeshFilter>());
        }
    }
    private void CreateLinePoint()
    {
        float button = InteractAction.ReadValue<float>();
        if (button != 0f)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(pointIndex++, rightController.position);
        }
    }

    void Update()
    {
        CreateLinePoint();
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
}
