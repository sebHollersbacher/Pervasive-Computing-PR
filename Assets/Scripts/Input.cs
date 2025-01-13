using System;
using UnityEngine;

public partial class Input
{
    private static Input _instance;
    
    public static Input Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }
    }
}
