using System.Collections.Generic;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;


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
    private SplineContainer splineContainer;
    private int pointIndex;

    public int segmentsPerUnit = 10; // Segments per unit of spline length
    public int radialSegments = 8;   // Segments around the cross-section (for a cylinder shape)
    public float radius = 0.01f;      // Radius of the circular cross-section
    private MeshFilter meshFilter;

    float timer;
    float timerDelay = 0.02f;

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
        //InteractAction.performed += InitDraw;
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
        currentParent = new GameObject("Line");
        splineContainer = currentParent.AddComponent<SplineContainer>();
        meshFilter = currentParent.AddComponent<MeshFilter>();


        SplineExtrude extrude = currentParent.AddComponent<SplineExtrude>();
        extrude.Container = splineContainer;
        extrude.Radius = radius;
        //extrude.
        currentParent.GetComponent<MeshRenderer>().material = Material;

        UpdateMesh();
    }

    void UpdateMesh()
    {
        Mesh extrudedMesh = GenerateExtrudedMesh(splineContainer.Spline);
        meshFilter.mesh = extrudedMesh;
    }

    Mesh GenerateExtrudedMesh(Spline spline)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float splineLength = spline.GetLength();
        int numSegments = Mathf.CeilToInt(splineLength * segmentsPerUnit);

        // Generate vertices and triangles for each segment along the spline
        for (int i = 0; i <= numSegments; i++)
        {
            float t = i / (float)numSegments;
            Vector3 pointOnSpline = spline.EvaluatePosition(t); // Position on spline
            Quaternion rotationOnSpline = Quaternion.LookRotation(spline.EvaluateTangent(t)); // Orientation on spline

            // Generate the circular cross-section at this point on the spline
            for (int j = 0; j < radialSegments; j++)
            {
                float angle = j * Mathf.PI * 2 / radialSegments;
                Vector3 localPos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                vertices.Add(pointOnSpline + rotationOnSpline * localPos);
            }

            // Create triangles between this segment and the previous one
            if (i > 0)
            {
                int startIdx = (i - 1) * radialSegments;
                int nextIdx = i * radialSegments;

                for (int j = 0; j < radialSegments; j++)
                {
                    int a = startIdx + j;
                    int b = startIdx + (j + 1) % radialSegments;
                    int c = nextIdx + j;
                    int d = nextIdx + (j + 1) % radialSegments;

                    // First triangle
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);

                    // Second triangle
                    triangles.Add(b);
                    triangles.Add(d);
                    triangles.Add(c);
                }
            }
        }

        // Create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }


    private void EndDraw2(InputAction.CallbackContext context)
    {
        UpdateMesh();
        SplineExtrude extrude = currentParent.GetComponent<SplineExtrude>();
        extrude.Container = splineContainer;
        extrude.Radius = radius;
        extrude.enabled = true;
        extrude.Rebuild();
        currentParent.GetComponent<MeshRenderer>().material = Material;
    }

    private void InitDraw(InputAction.CallbackContext context)
    {
        currentParent = new GameObject("Line");
        lineRenderer = currentParent.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = Material;
        lineRenderer.positionCount = 0;
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
            rightController.parent.Translate(0.2f, 0, .4f);
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
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount++, rightController.position);
                timer = timerDelay;
            }
        }
    }

    private void CreateKnot()
    {
        float button = InteractAction.ReadValue<float>();
        if (button != 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                splineContainer.Spline.Add(new BezierKnot(rightController.position), TangentMode.Linear);
                GenerateExtrudedMesh(splineContainer.Spline);
                //lineRenderer.SetPosition(lineRenderer.positionCount++, rightController.position);
                timer = timerDelay;
            }
        }
    }

    void Update()
    {
        CreateKnot();
        #region Move Controller
        if (DEBUG)
        {
            Vector3 input = MoveControllerAction.ReadValue<Vector3>();
            rightController.parent.Translate(input * Time.deltaTime);
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
