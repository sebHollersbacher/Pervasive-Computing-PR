using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public void Move(Vector3 difference)
    {
        gameObject.transform.position += difference;
    }

    public void Rotate(Quaternion difference)
    {
        gameObject.transform.rotation *= difference;
    }
}