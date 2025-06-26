using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputSystem;
    InputAction _action;
    [SerializeField] private float _sensitivity = 5f;
    private Vector2 _delta;

    private void OnEnable()
    {
        _action = _inputSystem["drag"];
        _action.Enable();
        _action.performed -= OnInputPerformed;
        _action.performed += OnInputPerformed;
    }

    private void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnInputPerformed;
    }

    private void OnInputPerformed(InputAction.CallbackContext context)
    {
        _delta = context.ReadValue<Vector2>();
        if (_delta != Vector2.zero)
        {
            transform.position += new Vector3(-_delta.x, -_delta.y, 0) * Time.deltaTime * _sensitivity;
        }
    }
}
