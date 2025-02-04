using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Vertex : MonoBehaviour
{
    private static readonly Material VertexMaterial = new(Shader.Find("Custom/TransparentShader"));
    public static void CreateVertex(int index, Vector3 position, Shapeable shapeable)
    {
        var selectionPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        selectionPoint.GetComponent<Renderer>().material = VertexMaterial;
        selectionPoint.GetComponent<Collider>().isTrigger = true;
        selectionPoint.transform.parent = shapeable.gameObject.transform;
        selectionPoint.transform.localPosition = position;
        selectionPoint.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
        
        var instance = selectionPoint.AddComponent<Vertex>();
        instance.SelectionPoint = selectionPoint;
        instance.Index = index;
        instance.Shapeable = shapeable;
        shapeable.Vertices.Add(instance);
    }
    
    public int Index { get; set; }
    public GameObject SelectionPoint { get; set; }

    private Shapeable _shapeable;
    public Shapeable Shapeable
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

    private void OnDestroy()
    {
        _shapeable.SetActiveEvent -= SelectionPoint.SetActive;
    }

    public void UpdateVisual()
    {
        SelectionPoint.transform.localPosition = Shapeable.Mesh.positions[Index];
    }

    public void Select()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(1,1,0,.4f);;
        Shapeable.SelectedVertices.Add(this);
        Shapeable.SelectedEdges.ToList().ForEach(edge => edge.Deselect());
    }

    public void Deselect()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(0,0,1,.4f);
        Shapeable.SelectedVertices.Remove(this);
    }
}
