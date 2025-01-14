using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Shapeable : MonoBehaviour
{
    public ProBuilderMesh Mesh { get; set; }
    
    public void Move(Vector3 difference)
    {
        var vertices = Mesh.GetVertices();
        var selected = SceneManager.Instance.GetShapeables().ToArray();
        var indices = selected.Select(v => v).ToDictionary(v => v.Index, v => v.SelectionPoint);
        for (int i = 0; i < vertices.Length; i++)
        {
            if (indices.ContainsKey(i))
            {
                var go = indices[i];
                go.transform.position += difference;
                vertices[i].position = go.transform.localPosition;
            }
        }
        
        Mesh.RebuildWithPositionsAndFaces(vertices.Select(v => v.position).ToArray(), Mesh.faces);
        Mesh.Refresh();
    }
}
