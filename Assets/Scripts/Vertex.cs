using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Vertex : MonoBehaviour
{
    public static void CreateVertex(GameObject selectionPoint, int index, Shapeable shapeable)
    {
        var instance = selectionPoint.AddComponent<Vertex>();
        instance.SelectionPoint = selectionPoint;
        instance.Index = index;
        instance.Shapeable = shapeable;
    }
    
    public int Index { get; set; }
    public GameObject SelectionPoint { get; set; }

    private Shapeable _shapeable;
    private Shapeable Shapeable
    {
        get => _shapeable;
        set
        {
            if (_shapeable != null)
            {
                _shapeable.SetActiveEvent -= SelectionPoint.SetActive;
            }

            _shapeable = value;

            if (_shapeable != null)
            {
                _shapeable.SetActiveEvent += SelectionPoint.SetActive;
            }
        }
    }

    public void Select()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(1,1,0,.4f);;

        // List<int> arrayList = new List<int>();
        // arrayList.AddRange(Shapeable.Mesh.selectedVertices);
        // arrayList.Add(Index);
        // Shapeable.Mesh.SetSelectedVertices(arrayList);
        // if (arrayList.Count >= 2)
        // {
            // int[] newint = ConnectElements.Connect(Shapeable.Mesh, Shapeable.Mesh.selectedVertices);
            // Shapeable.Mesh.edge
        // }
        Shapeable.vertices.Add(this);
    }

    public void Deselect()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(0,0,1,.4f);
        
        // List<int> arrayList = new List<int>();
        // arrayList.AddRange(Shapeable.Mesh.selectedVertices);
        // arrayList.Remove(Index);
        // Shapeable.Mesh.SetSelectedVertices(arrayList);
        Shapeable.vertices.Remove(this);
    }
}
