using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private void Awake()
    {
        if (gameObject.GetComponent<Outline>() == null)
        {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 10f;
            outline.enabled = false;
        }
    }

    public void Select()
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }

    public void Deselect()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }

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
        gameObject.transform.rotation = difference * gameObject.transform.rotation;
    }
    
    public void SetRotation(Quaternion roation)
    {
        gameObject.transform.rotation = roation;
    }
    
    public void Scale(Vector3 difference)
    {
        gameObject.transform.localScale += difference;
    }
}