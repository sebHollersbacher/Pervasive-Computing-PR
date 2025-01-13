using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShapingCollider : MonoBehaviour
{
    public List<Shapeable> shapeables = new();
    
    private void OnTriggerEnter(Collider other)
    {
        var shapeable = other.gameObject.GetComponent<Shapeable>();
        if(shapeable != null)
            shapeables.Add(shapeable);
    }

    private void OnTriggerExit(Collider other)
    {
        var shapeable = other.gameObject.GetComponent<Shapeable>();
        shapeables.Remove(shapeable);
    }
}