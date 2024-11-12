using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Drawing : MonoBehaviour
{
    public enum DrawMode
    {
        Cube,
        Line,
        Spline
    }

    private InputAction _interactAction;

    // General
    public DrawMode drawMode = DrawMode.Spline;
    public Transform brushTransform;
    public Material material;

    private GameObject _currentParent;
    private float _timer;
    private const float TimerDelay = 0.02f;

    // Splines
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private MeshFilter _meshFilter;

    public int segmentsPerUnit = 10;
    public int radialSegments = 4;
    public float radius = 0.02f;

    // Cubes
    public GameObject cube;
    private readonly List<MeshFilter> _meshes = new();

    // Lines
    private LineRenderer _lineRenderer;
    private int _pointIndex;

    #region Inputs

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();
        _interactAction.performed += InitDrawing;
        if (drawMode == DrawMode.Cube)
            _interactAction.canceled += EndDrawingCube;
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    #endregion

    private void Update()
    {
        switch (drawMode)
        {
            case DrawMode.Cube:
                CreateObject();
                break;
            case DrawMode.Line:
                CreateLinePoint();
                break;
            case DrawMode.Spline:
                CreateKnot();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region InitDrawing

    private void InitDrawing(InputAction.CallbackContext context)
    {
        switch (drawMode)
        {
            case DrawMode.Cube:
                InitDrawingCube();
                break;
            case DrawMode.Line:
                InitDrawingLine();
                break;
            case DrawMode.Spline:
                InitDrawingSpline();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void InitDrawingCube()
    {
        cube.GetComponent<MeshRenderer>().material = material;
        _currentParent = new GameObject("Line");
        _currentParent.AddComponent<MeshRenderer>().material = material;
    }
    private void InitDrawingLine()
    {
        _currentParent = new GameObject("Line");
        _lineRenderer = _currentParent.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.material = material;
        _lineRenderer.positionCount = 0;
    }

    private void InitDrawingSpline()
    {
        _currentParent = new GameObject("Line");

        _splineContainer = _currentParent.AddComponent<SplineContainer>();
        _meshFilter = _currentParent.AddComponent<MeshFilter>();
        _splineExtrude = _currentParent.AddComponent<SplineExtrude>();
        _currentParent.GetComponent<MeshRenderer>().material = material;

        _splineExtrude.enabled = true;
        _splineExtrude.Container = _splineContainer;
        _splineExtrude.Radius = radius;
        _splineExtrude.SegmentsPerUnit = segmentsPerUnit;
        _splineExtrude.Sides = radialSegments;
    }

    #endregion

    #region CreatePoints
    private void CreateObject()
    {
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;

        GameObject obj = Instantiate(cube, brushTransform.position, brushTransform.rotation,
            _currentParent.transform);
        _meshes.Add(obj.GetComponent<MeshFilter>());
    }

    private void CreateLinePoint()
    {
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;

        _timer -= Time.deltaTime;
        if (_timer > 0) return;
        _timer = TimerDelay;

        _lineRenderer.SetPosition(_lineRenderer.positionCount++, brushTransform.position);
    }

    private void CreateKnot()
    {
        var button = _interactAction.ReadValue<float>();
        if (button == 0f) return;

        _timer -= Time.deltaTime;
        if (_timer > 0) return;
        _timer = TimerDelay;

        var knot = new BezierKnot(brushTransform.position)
        {
            Rotation = brushTransform.rotation
        };
        _splineContainer.Spline.Add(knot, TangentMode.Linear);
        GenerateExtrudedMesh();
    }

    #endregion

    private void GenerateExtrudedMesh()
    {
        Spline spline = _splineContainer.Spline;
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
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;

        _splineExtrude.Rebuild();
    }

    private void EndDrawingCube(InputAction.CallbackContext context)
    {
        CombineInstance[] combine = new CombineInstance[_meshes.Count];
        for (int i = 0; i < _meshes.Count; i++)
        {
            combine[i].mesh = _meshes[i].sharedMesh;
            combine[i].transform = _meshes[i].transform.localToWorldMatrix;
            _meshes[i].gameObject.SetActive(false);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);
        _currentParent.AddComponent<MeshFilter>().mesh = combinedMesh;
        _meshes.Clear();
    }
}