using System.Collections.Generic;
using System.Linq;

public class SceneManager
{
    private static SceneManager _instance;
    
    private readonly HashSet<Shapeable> _shapeables = new();
    private readonly HashSet<Selectable> _selectables = new();
    
    public static SceneManager Instance
    {
        get { return _instance ??= new SceneManager(); }
    }

    public List<Selectable> GetSelectables()
    {
        return _selectables.ToList();
    }
    public List<Shapeable> GetShapeables()
    {
        return _shapeables.ToList();
    }

    public void Add(Selectable selectable)
    {
        _selectables.Add(selectable);
    }
    public void Add(Shapeable shapeable)
    {
        _shapeables.Add(shapeable);
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
        foreach (var shapeable in _shapeables)
        {
            shapeable.vertices.ToList().ForEach(vertex => vertex.Deselect());
        }

        _shapeables.Clear();
    }
}
