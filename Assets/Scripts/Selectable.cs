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
        Vector3 localMovement = gameObject.transform.InverseTransformDirection(difference);
        Vector3 scaleChange = new Vector3(
            Mathf.Sign(localMovement.x) * Mathf.Abs(localMovement.x),
            Mathf.Sign(localMovement.y) * Mathf.Abs(localMovement.y),
            Mathf.Sign(localMovement.z) * Mathf.Abs(localMovement.z)
        );
        
        Vector3 newScale = gameObject.transform.localScale + scaleChange;
        newScale.x = Mathf.Max(newScale.x, 0);
        newScale.y = Mathf.Max(newScale.y, 0);
        newScale.z = Mathf.Max(newScale.z, 0);

        gameObject.transform.localScale = newScale;
    }
}