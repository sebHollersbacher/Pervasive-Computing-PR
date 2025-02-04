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
        
        foreach (var vertex in Vertices)
        {
            vertex.UpdateVisual();
        }
        
        foreach (var edge in Edges)
        {
            edge.UpdateVisual();
        }
    }

    public void CreateEdges()
    {
        var indices = ConnectElements.Connect(Mesh, SelectedVertices.Select(v => v.Index).ToList());
        Mesh.Refresh();
        
        RefreshEdges();
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
        Mesh.AppendVerticesToEdge(SelectedEdges.Select(edge => edge.edge).ToList(), 1);
        
        RefreshEdges();
    }

    private void RefreshEdges()
    {
        SelectedVertices.Clear();
        Vertices.ToList().ForEach(vertex => Destroy(vertex.gameObject));
        Vertices.Clear();
        
        SelectedEdges.Clear();
        Edges.ToList().ForEach(edge => Destroy(edge.gameObject));
        Edges.Clear();

        Mesh.faces.SelectMany(face => face.edges.Select(edge => edge)).ToList().ForEach(edge =>
        {
            Edge.CreateEdge(edge,this);
        });
    }
}