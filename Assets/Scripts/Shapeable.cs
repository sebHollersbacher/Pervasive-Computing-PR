using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Shapeable : MonoBehaviour
{
    public delegate void ActiveDelegate(bool active);

    public event ActiveDelegate SetActiveEvent;
    public ProBuilderMesh Mesh { get; set; }

    public HashSet<Edge> Edges = new();
    public HashSet<Edge> SelectedEdges = new();

    public HashSet<Vertex> Vertices = new();
    public HashSet<Vertex> SelectedVertices = new();

    private void Awake()
    {
        SceneManager.Instance.Add(this);
    }

    public void SetActive(bool active)
    {
        SetActiveEvent?.Invoke(active);
    }

    public void Move(Vector3 difference)
    {
        var meshVertices = Mesh.GetVertices();
        var indices = SelectedVertices.Select(v => v.Index).ToList();
        SelectedEdges.ToList().ForEach(edge =>
        {
            edge.Shapeable.Mesh.GetCoincidentVertices(new List<int> { edge.edge.a, edge.edge.a })
                .ForEach(index => indices.Add(index));
        });
        for (int i = 0; i < meshVertices.Length; i++)
        {
            if (indices.Contains(i))
            {
                meshVertices[i].position += gameObject.transform.InverseTransformVector(difference);
            }
        }

        SelectedVertices.Select(v => v.SelectionPoint).ToList().ForEach(go => go.transform.position += difference);
        SelectedEdges.Select(v => v.SelectionPoint).ToList().ForEach(go => go.transform.position += difference);

        Mesh.RebuildWithPositionsAndFaces(meshVertices.Select(v => v.position).ToArray(), Mesh.faces);
        Mesh.Refresh();
    }

    public void CreateEdges()
    {
        var indices = ConnectElements.Connect(Mesh, SelectedVertices.Select(v => v.Index).ToList());
        Mesh.Refresh();
        RefreshVertices();
        // RefreshEdges();
        List<int> list = new();
        foreach (var index in indices)
        {
            Mesh.GetCoincidentVertices(index, list);
        }

        Vertices.ToList().ForEach(v =>
        {
            if (list.Contains(v.Index))
            {
                v.Select();
            }
        });
    }

    public void CreateVertices()
    {
        SelectedEdges.ToList().ForEach(edge =>
        {
            var newEdges = Mesh.AppendVerticesToEdge(edge.edge, 1);
        });
        RefreshVertices();
        RefreshEdges();
    }

    private void RefreshVertices()
    {
        SelectedVertices.Clear();
        Vertices.ToList().ForEach(vertex => Destroy(vertex.gameObject));
        Vertices.Clear();

        Material vertexMaterial = new(Shader.Find("Custom/TransparentShader"));
        var vertices = Mesh.positions;
        for (int i = 0; i < vertices.Count; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            c.GetComponent<Renderer>().material = vertexMaterial;
            c.transform.parent = Mesh.gameObject.transform;
            c.transform.localPosition = vertices[i];
            c.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Vertex.CreateVertex(c, i, this);
            c.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void RefreshEdges()
    {
        Debug.Log(SelectedEdges.Count);
        SelectedEdges.Clear();
        Edges.ToList().ForEach(vertex => Destroy(vertex.gameObject));
        Edges.Clear();

        Material vertexMaterial = new(Shader.Find("Custom/TransparentShader"));

        Mesh.faces.SelectMany(face => face.edges.Select(edge => edge)).ToList().ForEach(edge =>
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.GetComponent<Renderer>().material = vertexMaterial;
            c.transform.parent = Mesh.gameObject.transform;
            c.transform.localPosition = (Mesh.positions[edge.a] + Mesh.positions[edge.b])/2f;
            c.transform.localScale = new Vector3(0.05f, 0.05f, (Mesh.positions[edge.b] - Mesh.positions[edge.a]).magnitude);
            c.transform.localRotation = Quaternion.LookRotation(Mesh.positions[edge.b] - Mesh.positions[edge.a]);
            Edge.CreateEdge(c,edge,this);
            c.GetComponent<Collider>().isTrigger = true;
        });
    }
    
    private void RefreshEdges(List<UnityEngine.ProBuilder.Edge> edges)
    {
        SelectedEdges.Clear();

        Material vertexMaterial = new(Shader.Find("Custom/TransparentShader"));
        foreach (var edge in edges)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.GetComponent<Renderer>().material = vertexMaterial;
            c.transform.parent = Mesh.gameObject.transform;
            c.transform.localPosition = (Mesh.positions[edge.a] + Mesh.positions[edge.b]) / 2f;
            c.transform.localScale =
                new Vector3(0.05f, 0.05f, (Mesh.positions[edge.b] - Mesh.positions[edge.a]).magnitude);
            c.transform.localRotation = Quaternion.LookRotation(Mesh.positions[edge.b] - Mesh.positions[edge.a]);
            Edge.CreateEdge(c, edge, this);
            c.GetComponent<Collider>().isTrigger = true;
        }
    }
}