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
    private GameObject currentParent1 = null;
    private GameObject currentParent2 = null;
    public Material Material;

    private List<Vector3> linePoints = new List<Vector3>();
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private int pointIndex;

    float timer;
    float timerDelay = 0.01f;

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
        InteractAction.performed += InitDraw2;
        InteractAction.canceled += EndDraw2;
    }

    private void OnDisable()
    {
        MoveControllerAction.Disable();
        LookMouseAction.Disable();
        InteractAction.Disable();
    }

    private void InitDraw2(InputAction.CallbackContext context)
    {
    }
    private void EndDraw2(InputAction.CallbackContext context)
    {
        currentParent1 = new GameObject("Line");
        lineRenderer1 = currentParent1.AddComponent<LineRenderer>();
        lineRenderer1.material = Material;
        lineRenderer1.startWidth = 0.01f;
        lineRenderer1.endWidth = 0.01f;

        lineRenderer1.positionCount = linePoints.Count;
        lineRenderer1.SetPositions(linePoints.ToArray());

        linePoints.Clear();
    }

    private void InitDraw(InputAction.CallbackContext context)
    {
        currentParent1 = new GameObject("Line");
        lineRenderer1 = currentParent1.AddComponent<LineRenderer>();
        lineRenderer1.startWidth = 0.1f;
        lineRenderer1.endWidth = 0.1f;
        lineRenderer1.material = Material;
        lineRenderer1.positionCount = 0;
        lineRenderer1.alignment = LineAlignment.TransformZ;

        currentParent2 = new GameObject("Line");
        currentParent2.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
        lineRenderer2 = currentParent2.AddComponent<LineRenderer>();
        lineRenderer2.startWidth = 0.02f;
        lineRenderer2.endWidth = 0.02f;
        lineRenderer2.material = Material;
        lineRenderer2.positionCount = 0;
        lineRenderer2.alignment = LineAlignment.TransformZ;
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
        currentParent1.AddComponent<MeshFilter>().mesh = combinedMesh;
        currentParent1.AddComponent<MeshRenderer>().material = Material;
        meshes.Clear();
    }

    void Start()
    {
        timer = timerDelay;
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
                currentParent1.transform);
            meshes.Add(obj.GetComponent<MeshFilter>());
        }
    }
    private void CreateLinePoint()
    {
        float button = InteractAction.ReadValue<float>();
        if (button != 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                linePoints.Add(rightController.position);
                timer = timerDelay;
            }
            //lineRenderer1.positionCount++;
            //lineRenderer1.SetPosition(pointIndex, Quaternion.Euler(0,0, rightController.rotation.z) * rightController.position);
            //lineRenderer2.positionCount++;
            //lineRenderer2.SetPosition(pointIndex++, Quaternion.Euler(0, 0, rightController.rotation.z) * rightController.position);
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
