using UnityEngine;

public partial class Input : MonoBehaviour
{
    private static Input _instance = null;
    
    public static Input Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Input();
            }
            return _instance;
        }
    }
}
