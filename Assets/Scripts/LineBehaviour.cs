using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<int> points = new();

    public void AddPoint(int point)
    {
        points.Add(point);
    }

    public int GetPointIndex(int point)
    {
        return points.IndexOf(point);
    }

    public void RemovePoint(int index)
    {
        if (index < 0 || index >= points.Count) return;
        points.RemoveAt(index);
    }
}
