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
        if (triangles.Count < count * index) return;

        triangles.RemoveRange(count * (index - 1), count);
        gameObject.GetComponentInParent<MeshFilter>().sharedMesh.triangles = triangles.ToArray();
    }
}