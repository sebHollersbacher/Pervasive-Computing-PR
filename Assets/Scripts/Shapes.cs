using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Shapes : MonoBehaviour
{
    private InputAction _interactAction;
    private bool _inputEnabled;
    private bool _isCreating;

    public Transform creationPoint;
    private GameObject _shape;
    private Vector3 _initPoint;
    public Color ShapeColor { get; set; } = Color.red;
    public ShapeType SelectedShapeType { get; set; } = ShapeType.Cube;
    public float Radius { get; set; } = 0.03f;

    public enum ShapeType
    {
        Line,
        Plane,
        Cube,
        Sphere,
        Cylinder,
        Pyramid
    }

    #region Inputs

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
        _interactAction.Enable();

        _interactAction.performed += InitCreateShape;
        _interactAction.canceled += FinishCreateShape;
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    public void DisableInputs()
    {
        _interactAction.Disable();
        _inputEnabled = false;
    }

    public void EnableInputs()
    {
        _inputEnabled = true;
        _interactAction.Enable();
    }

    #endregion

    private void FixedUpdate()
    {
        if (!_inputEnabled || !_isCreating) return;
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;

        var size = Vector3.Magnitude(creationPoint.transform.position - _initPoint);
        _shape.transform.localScale = new Vector3(size, size, size);

        Vector3 newPoint = creationPoint.transform.position;
        _shape.transform.rotation = Quaternion.LookRotation(newPoint - _initPoint) * Quaternion.Euler(90, 0, 0);
        if (SelectedShapeType == ShapeType.Plane)
        {
            _shape.transform.localScale = new Vector3(0.001f, size, size);
        }
        if (SelectedShapeType == ShapeType.Line)
        {
            _shape.transform.localScale = new Vector3(2*Radius, size/2, 2*Radius);
            _shape.transform.position = (_initPoint + creationPoint.transform.position) * 0.5f;
        }
    }

    private void InitCreateShape(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) return;
        _isCreating = true;

        switch (SelectedShapeType)
        {
            case ShapeType.Line:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                _shape.name = "Line";
                break;
            case ShapeType.Plane:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _shape.name = "Plane";
                break;
            case ShapeType.Cube:
                var proBuilderMesh = ShapeGenerator.GenerateCube(PivotLocation.Center, Vector3.one);
                ConnectElements.Connect(proBuilderMesh,proBuilderMesh.faces);   // Subdivide Object
                // ConnectElements.Connect(proBuilderMesh,proBuilderMesh.faces);
                // Refresh the mesh to apply changes
                proBuilderMesh.ToMesh();
                proBuilderMesh.Refresh();
                _shape = proBuilderMesh.gameObject;
                var meshCollider = _shape.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = _shape.GetComponent<MeshFilter>().sharedMesh;
                meshCollider.convex = true;

                var shapeable = _shape.AddComponent<Shapeable>();
                shapeable.Mesh = proBuilderMesh;
                
                var vertices = proBuilderMesh.GetVertices();
                for(int i = 0; i < vertices.Length; i++)
                {
                    var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.parent = _shape.transform;
                    c.transform.position = vertices[i].position;
                    c.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    var vertex = c.AddComponent<Vertex>();
                    vertex.Index = i;
                    vertex.SelectionPoint = c;
                    vertex.Shapeable = shapeable;
                    c.GetComponent<Collider>().isTrigger = true;
                }
                _shape.name = "BuilderCube";
                break;
            case ShapeType.Sphere:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case ShapeType.Cylinder:
                _shape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
            case ShapeType.Pyramid:
                _shape = CreatePyramid();
                break;
        }

        Material material = new(Shader.Find("Standard"));
        material.color = ShapeColor;
        _shape.GetComponent<MeshRenderer>().material = material;
        _shape.transform.position = creationPoint.transform.position;
        _shape.transform.localScale = Vector3.zero;
        _initPoint = creationPoint.transform.position;
    }

    private void FinishCreateShape(InputAction.CallbackContext ctx)
    {
        if (!_inputEnabled) return;
        _shape.GetComponent<Collider>().isTrigger = true;
        _shape.AddComponent<Selectable>();
        _isCreating = false;
    }
    
    private GameObject CreatePyramid()
    {
        GameObject pyramid = new("Pyramid");
        Mesh mesh = new Mesh();
        pyramid.AddComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices =
        {
            new(0, 1, 0),
            new(-0.5f, 0, -0.5f),
            new(0.5f, 0, -0.5f),
            new(0.5f, 0, 0.5f),
            new(-0.5f, 0, 0.5f)
        };

        int[] triangles =
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3,
            0, 1, 4,
            1, 2, 3,
            1, 3, 4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        pyramid.AddComponent<MeshRenderer>();
        var meshCollider = pyramid.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        
        return pyramid;
    }
}