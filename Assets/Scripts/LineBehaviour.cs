using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<int> _points = new();

    public void AddPoint(int point)
    {
        _points.Add(point);
    }

    public int GetPointIndex(int point)
    {
        return _points.IndexOf(point);
    }

    public void RemovePoint(int index)
    {
        if (index < 0 || index >= _points.Count) return;
        _points.RemoveAt(index);
    }
}
