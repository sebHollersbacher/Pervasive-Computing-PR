using UnityEngine;
using UnityEngine.InputSystem;

public class Erasing : MonoBehaviour
{
    private InputAction _interactAction;
    private bool _inputEnabled = true;

    public GameObject eraser;
    
    #region Inputs

    private void OnEnable()
    {
        _interactAction = Input.Instance.User.Interact;
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    public void DisableInputs()
    {
        _inputEnabled = false;
        eraser.SetActive(false);
        _interactAction.Disable();
    }

    public void EnableInputs()
    {
        _interactAction.Enable();
        eraser.SetActive(true);
        _inputEnabled = true;
    }
    
    #endregion

    private void FixedUpdate()
    {
        if (!_inputEnabled) return;
        float button = _interactAction.ReadValue<float>();
        if (button == 0f) return;
    }
}