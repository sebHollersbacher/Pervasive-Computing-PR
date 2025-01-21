using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Shapeable : MonoBehaviour
{
    public delegate void ActiveDelegate(bool active);
    public event ActiveDelegate SetActiveEvent;
    public ProBuilderMesh Mesh { get; set; }
    
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
        var indices = SelectedVertices.Select(v => v).ToDictionary(v => v.Index, v => v.SelectionPoint);
        for (int i = 0; i < meshVertices.Length; i++)
        {
            if (indices.ContainsKey(i))
            {
                var go = indices[i];
                go.transform.position += difference;
                meshVertices[i].position = go.transform.localPosition;
            }
        }
        
        Mesh.RebuildWithPositionsAndFaces(meshVertices.Select(v => v.position).ToArray(), Mesh.faces);
        Mesh.Refresh();
    }

    public void CreateEdges()
    {
        var indices = ConnectElements.Connect(Mesh, SelectedVertices.Select(v => v.Index).ToList());
        Mesh.Refresh();
        RefreshVertices();
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

    private void RefreshVertices()
    {
        SelectedVertices.Clear();
        Vertices.ToList().ForEach(vertex => Destroy(vertex.gameObject));
        Vertices.Clear();
        
        Material vertexMaterial = new(Shader.Find("Custom/TransparentShader"));
        var vertices = Mesh.positions;
        for(int i = 0; i < vertices.Count; i++)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            c.GetComponent<Renderer>().material = vertexMaterial;
            c.transform.parent = Mesh.gameObject.transform;
            c.transform.localPosition = vertices[i];
            c.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Vertex.CreateVertex(c,i,this);
            c.GetComponent<Collider>().isTrigger = true;
        }
    }
}
