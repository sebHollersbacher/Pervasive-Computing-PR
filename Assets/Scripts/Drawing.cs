using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drawing : MonoBehaviour
{
    private InputAction _interactAction;

    // General
    public GameObject brush;
    private Color _lineColor = Color.red;

    private GameObject _currentParent;
    private bool _inputEnabled = true;

    public int radialSegments = 4;
    private float radius = 0.03f;

    public GameObject colliderPrefab;

    // Mesh
    private Mesh _mesh;
    private readonly List<Vector3> _vertices = new();
    private readonly List<int> _triangles = new();
    private Vector3 _prevPoint;
    private Quaternion _prevRotation = Quaternion.identity;
    private LineBehaviour _lineBehaviour;

    #region Inputs

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();
        _interactAction.performed += InitDrawingMesh;
        _interactAction.canceled += FinishDrawingMesh;
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    public void DisableInputs()
    {
        _inputEnabled = false;
        brush.SetActive(false);
        _interactAction.Disable();
    }

    public void EnableInputs()
    {
        _interactAction.Enable();
        brush.SetActive(true);
        _inputEnabled = true;
    }

    public void ChangeLineColor(Color newColor)
    {
        _lineColor = newColor;
        brush.GetComponent<MeshRenderer>().material.color = _lineColor;
    }
    
    public void ChangeRadius(float newRadius)
    {
        radius = newRadius;
        brush.transform.localScale = new Vector3(2*radius, 2*radius, 2*radius);
    }

    #endregion

    private void FixedUpdate()
    {
        if (!_inputEnabled) return;
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;
        UpdateMesh();
    }

    #region InitDrawing

    private void InitDrawingMesh(InputAction.CallbackContext context)
    {
        _vertices.Clear();
        _triangles.Clear();
        _currentParent = new GameObject("Line");
        _lineBehaviour = _currentParent.AddComponent<LineBehaviour>();
        _currentParent.AddComponent<Selectable>();

        Material mat = new(Shader.Find("Standard"));
        mat.color = _lineColor;
        _currentParent.AddComponent<MeshRenderer>().material = mat;
        _mesh = _currentParent.AddComponent<MeshFilter>().mesh;
        _mesh.MarkDynamic();

        _prevPoint = brush.transform.position;
    }
    
    private void FinishDrawingMesh(InputAction.CallbackContext context)
    {
        var meshCollider = _currentParent.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = _mesh;
        meshCollider.convex = true; // TODO: line is not convex (See SegmentCollider for right collision detection)
        meshCollider.isTrigger = true;
    }

    #endregion

    void UpdateMesh()
    {
        _mesh.Clear();
        GenerateExtendedMesh();

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    private void GenerateExtendedMesh()
    {
        Vector3 newPoint = brush.transform.position;
        Vector3 direction = (newPoint - _prevPoint);
        if (direction.magnitude < 0.003f) return;
        Quaternion newRotation = Quaternion.LookRotation((_prevPoint - newPoint).normalized, Vector3.up);
        Quaternion newRotationInv = Quaternion.LookRotation(direction.normalized, Vector3.up);

        if (_prevRotation == Quaternion.identity)
        {
            // first face should be same as next
            AddFace(_prevPoint, newRotation);
        }
        else
        {
            // rotations of first face of sector should be prevRotation
            AddFace(_prevPoint, _prevRotation);
        }

        int ringStartIdx = _vertices.Count + 1;
        AddFace(newPoint, newRotationInv, true);
        int prevRingStartIdx = ringStartIdx - radialSegments - 2;
        for (int j = 0; j < radialSegments; j++)
        {
            int c = ringStartIdx + j;
            int d = ringStartIdx + (j + 1) % radialSegments;
            int a = prevRingStartIdx + radialSegments - ((j + 2) % radialSegments);
            int b = prevRingStartIdx + radialSegments - ((j + 3) % radialSegments);

            _triangles.Add(a);
            _triangles.Add(b);
            _triangles.Add(c);

            _triangles.Add(b);
            _triangles.Add(d);
            _triangles.Add(c);
        }

        _prevPoint = newPoint;
        _prevRotation = newRotation;
    }

    private void AddFace(Vector3 position, Quaternion rotation, bool createCol = false)
    {
        int middle = _vertices.Count;

        _vertices.Add(position);
        for (int j = 0; j < radialSegments; j++)
        {
            float angle = j * Mathf.PI * 2 / radialSegments;
            Vector3 localPos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            _vertices.Add(position + rotation * localPos);
        }

        int startIndex = _vertices.Count - radialSegments;
        for (int j = 0; j < radialSegments; j++)
        {
            int current = startIndex + j;
            int next = startIndex + (j + 1) % radialSegments;

            _triangles.Add(middle);
            _triangles.Add(current);
            _triangles.Add(next);
        }

        if (createCol)
        {
            GameObject vertexMarker = Instantiate(colliderPrefab, position, Quaternion.identity);
            vertexMarker.GetComponent<SegmentCollider>().index = _vertices.Count / (radialSegments*2 + 2);
            _lineBehaviour.AddPoint(_vertices.Count / (radialSegments*2 + 2));
            vertexMarker.transform.parent = _currentParent.transform;
        }
    }
}