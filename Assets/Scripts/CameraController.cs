using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    #region Define
    const string DragAction = "Drag";
    const string ZoomAction = "Zoom";
    #endregion

    #region Inspector
    [SerializeField] private InputActionAsset _inputSystem;
    private InputAction _drag;
    private InputAction _zoom;

    [SerializeField] private float _sensitivity = 5f;
    private Vector2 _delta;
    private float _scrollY;
    
    private CinemachineVirtualCamera _camera;

    #endregion

    private void OnEnable()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();

        _drag = _inputSystem[DragAction];
        _drag.performed -= OnDragPerformed;
        _drag.performed += OnDragPerformed;

        _zoom = _inputSystem[ZoomAction];
        _zoom.performed -= OnZoomPerformed;
        _zoom.performed += OnZoomPerformed;
    }

    private void OnDisable()
    {
        _drag.performed -= OnDragPerformed;
        _zoom.performed -= OnZoomPerformed;
    }

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI() || EditState.IsDraggingBuilding) return; // UI 위에 있을 때는 드래그 무시

        _delta = context.ReadValue<Vector2>();
        if (_delta != Vector2.zero)
        {
            transform.position += new Vector3(-_delta.x, -_delta.y, 0) * Time.deltaTime * _sensitivity;
        }
    }

    private void OnZoomPerformed(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI()) return; // UI 위에 있을 때는 드래그 무시

        _scrollY = context.ReadValue<float>();
        
        if (_scrollY != 0f)
        {
            var zoom = _camera.m_Lens.OrthographicSize + _scrollY * 0.02f;
            _camera.m_Lens.OrthographicSize = Mathf.Clamp(zoom, 1f, 10f);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
