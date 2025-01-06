using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public void Move(Vector3 difference)
    {
        gameObject.transform.position += difference;
    }
    
    public void SetXPosition(float value)
    {
        Vector3 temp = gameObject.transform.position;
        temp.x = value;
        gameObject.transform.position = temp;
    }

    public void SetYPosition(float value)
    {
        Vector3 temp = gameObject.transform.position;
        temp.y = value;
        gameObject.transform.position = temp;
    }

    public void SetZPosition(float value)
    {
        Vector3 temp = gameObject.transform.position;
        temp.z = value;
        gameObject.transform.position = temp;
    }

    public void Rotate(Quaternion difference)
    {
        gameObject.transform.rotation *= difference;
    }
    
    public void SetRotation(Quaternion rotation)
    {
        gameObject.transform.rotation = rotation;
    }
    
    public void Scale(Vector3 difference)
    {
        gameObject.transform.localScale += difference;
    }
}