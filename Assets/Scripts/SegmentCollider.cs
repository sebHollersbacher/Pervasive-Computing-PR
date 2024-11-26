using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentCollider : MonoBehaviour
{
    [SerializeField] public int index;

    private int segmentsCount = 8;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Eraser") return;

        Mesh mesh = gameObject.GetComponentInParent<MeshFilter>().sharedMesh;
        List<int> triangles = new(mesh.triangles);
        int count = 12 * segmentsCount;
        LineBehaviour lineBehaviour = gameObject.GetComponentInParent<LineBehaviour>();
        int idx = lineBehaviour.GetPointIndex(index);
        if (idx < 0 || triangles.Count < count * (idx+1)) return;

        lineBehaviour.RemovePoint(idx);
        triangles.RemoveRange(count * idx, count);
        gameObject.GetComponentInParent<MeshFilter>().sharedMesh.triangles = triangles.ToArray();
    }
}