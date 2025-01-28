using System.Linq;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public static void CreateEdge(GameObject selectionPoint, UnityEngine.ProBuilder.Edge edge, Shapeable shapeable)
    {
        var instance = selectionPoint.AddComponent<Edge>();
        instance.SelectionPoint = selectionPoint;
        instance.edge = edge;
        instance.Shapeable = shapeable;
        shapeable.Edges.Add(instance);
    }
    
    public UnityEngine.ProBuilder.Edge edge { get; set; }
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

    public void Select()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(1,1,0,.4f);
        Shapeable.SelectedEdges.Add(this);
        Shapeable.SelectedVertices.ToList().ForEach(vertex => vertex.Deselect());
    }

    public void Deselect()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(0,0,1,.4f);
        Shapeable.SelectedEdges.Remove(this);
    }
}
