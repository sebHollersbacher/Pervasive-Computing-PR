using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Shapeable : MonoBehaviour
{
    public int Index { get; set; }
    
    public GameObject SelectionPoint { get; set; }
    public ProBuilderMesh Mesh { get; set; }

    public void Move(Vector3 difference)
    {
        var vertices = Mesh.GetVertices();
        var vertex = (Vertex)vertices.GetValue(Index);
        vertices.SetValue(vertex, Index);
        
        SelectionPoint.transform.position += difference;
        vertex.position = SelectionPoint.transform.localPosition;
        
        Mesh.RebuildWithPositionsAndFaces(vertices.Select(v => v.position).ToArray(), Mesh.faces);
        Mesh.Refresh();
    }
}
