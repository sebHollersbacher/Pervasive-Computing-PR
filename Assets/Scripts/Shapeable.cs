using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Shapeable : MonoBehaviour
{
    public delegate void ActiveDelegate(bool active);
    public event ActiveDelegate SetActiveEvent;
    public ProBuilderMesh Mesh { get; set; }
    
    public HashSet<Vertex> vertices = new();

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
        var indices = vertices.Select(v => v).ToDictionary(v => v.Index, v => v.SelectionPoint);
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
}
