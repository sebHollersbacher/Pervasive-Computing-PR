using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Drawing : MonoBehaviour
{
    private Input input;
    private InputAction InteractAction;

    // Splines
    public Material Material;
    private GameObject currentParent = null;
    private SplineContainer splineContainer;
    private SplineExtrude splineExtrude;

    public int segmentsPerUnit = 10;
    public int radialSegments = 10;
    public float radius = 0.02f;
    private MeshFilter meshFilter;

    public Transform brushTransform;

    // Cubes
    public GameObject cube;
    private List<MeshFilter> meshes = new List<MeshFilter>();

    // Lines
    private List<Vector3> linePoints = new List<Vector3>();
    private LineRenderer lineRenderer;
    private int pointIndex;

    float timer;
    float timerDelay = 0.02f;

    #region Inputs

    private void OnEnable()
    {
        InteractAction = Input.Instance.User.Interact;
        InteractAction.Enable();
        InteractAction.performed += InitDrawing;
    }

    private void OnDisable()
    {
        InteractAction.Disable();
    }

    #endregion

    private void Update()
    {
        CreateKnot();
    }

    private void InitDrawing(InputAction.CallbackContext context)
    {
        currentParent = new GameObject("Line");
        
        splineContainer = currentParent.AddComponent<SplineContainer>();
        meshFilter = currentParent.AddComponent<MeshFilter>();
        splineExtrude = currentParent.AddComponent<SplineExtrude>();
        currentParent.GetComponent<MeshRenderer>().material = Material;
        
        splineExtrude.enabled = true;
        splineExtrude.Container = splineContainer;
        splineExtrude.Radius = radius;
        splineExtrude.SegmentsPerUnit = segmentsPerUnit;
        splineExtrude.Sides = radialSegments;
    }

    private void CreateKnot()
    {
        var button = InteractAction.ReadValue<float>();
        if (button == 0f) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;
        timer = timerDelay;

        splineContainer.Spline.Add(new BezierKnot(brushTransform.position), TangentMode.Linear);
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        meshFilter.mesh = GenerateExtrudedMesh(splineContainer.Spline);
        splineExtrude.Rebuild();
    }

    private Mesh GenerateExtrudedMesh(Spline spline)
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

    #region Other

    /*
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
    }*/

    #endregion
}