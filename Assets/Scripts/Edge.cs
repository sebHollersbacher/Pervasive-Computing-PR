using System.Linq;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private static readonly Material EdgeMaterial = new(Shader.Find("Custom/TransparentShader"));
    public static void CreateEdge(UnityEngine.ProBuilder.Edge edge, Shapeable shapeable)
    {
        var proBuilderMesh = shapeable.Mesh;
        
        var selectionPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        selectionPoint.GetComponent<Collider>().isTrigger = true;
        selectionPoint.GetComponent<Renderer>().material = EdgeMaterial;
        selectionPoint.transform.parent = shapeable.gameObject.transform;
        selectionPoint.transform.localPosition = (proBuilderMesh.positions[edge.a] + proBuilderMesh.positions[edge.b])/2f;
        selectionPoint.transform.localScale = new Vector3(0.03f, 0.03f, (proBuilderMesh.positions[edge.b] - proBuilderMesh.positions[edge.a]).magnitude);
        selectionPoint.transform.localRotation = Quaternion.LookRotation(proBuilderMesh.positions[edge.b] - proBuilderMesh.positions[edge.a]);
        
        var instance = selectionPoint.AddComponent<Edge>();
        instance.SelectionPoint = selectionPoint;
        instance.edge = edge;
        instance.Shapeable = shapeable;
        shapeable.Edges.Add(instance);
        
        Vertex.CreateVertex(edge.a, proBuilderMesh.positions[edge.a], shapeable);
        Vertex.CreateVertex(edge.b, proBuilderMesh.positions[edge.b], shapeable);
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
    
    public void UpdateVisual()
    {
        var proBuilderMesh = Shapeable.Mesh;
        SelectionPoint.transform.localPosition = (proBuilderMesh.positions[edge.a] + proBuilderMesh.positions[edge.b])/2f;
        SelectionPoint.transform.localScale = new Vector3(0.03f, 0.03f, (proBuilderMesh.positions[edge.b] - proBuilderMesh.positions[edge.a]).magnitude);
        SelectionPoint.transform.localRotation = Quaternion.LookRotation(proBuilderMesh.positions[edge.b] - proBuilderMesh.positions[edge.a]);
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
