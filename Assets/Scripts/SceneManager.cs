using System.Collections.Generic;
using System.Linq;

public class SceneManager
{
    private static SceneManager _instance;
    
    private readonly HashSet<VertexIndex> _vertices = new();    // TODO: remove this and move to Shapeable
    private readonly HashSet<Selectable> _selectables = new();
    
    public static SceneManager Instance
    {
        get { return _instance ??= new SceneManager(); }
    }

    public List<Selectable> GetSelectables()
    {
        return _selectables.ToList();
    }
    public List<VertexIndex> GetShapeables()
    {
        return _vertices.ToList();
    }

    public void Add(Selectable selectable)
    {
        _selectables.Add(selectable);
    }
    public void Add(VertexIndex vertex)
    {
        vertex.Select();
        _vertices.Add(vertex);
    }

    public void ClearSelectables()
    {
        foreach (var selectable in _selectables)
        {
            selectable.Deselect();
        }
        
        _selectables.Clear();
    }
    public void ClearShapeables()
    {
        foreach (var vertexIndex in _vertices)
        {
            vertexIndex.Deselect();
        }

        _vertices.Clear();
    }
}
