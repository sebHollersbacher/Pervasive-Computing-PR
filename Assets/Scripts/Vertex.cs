using UnityEngine;

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

        Shapeable.vertices.Add(this);
    }

    public void Deselect()
    {
        SelectionPoint.GetComponent<Renderer>().material.color = new Color(0,0,1,.4f);
        
        Shapeable.vertices.Remove(this);
    }
}
