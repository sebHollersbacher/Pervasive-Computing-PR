using UnityEngine;

public class Vertex : MonoBehaviour
{
    public int Index { get; set; }
    public GameObject SelectionPoint { get; set; }
    
    public Shapeable Shapeable { get; set; }
    
    public void Select()
    {
        Material material = new(Shader.Find("Standard"));
        material.color = Color.blue;
        SelectionPoint.GetComponent<Renderer>().material = material;

        Shapeable.vertices.Add(this);
    }

    public void Deselect()
    {
        Material material = new(Shader.Find("Standard"));
        material.color = Color.white;
        SelectionPoint.GetComponent<Renderer>().material = material;
        
        Shapeable.vertices.Remove(this);
    }
}
