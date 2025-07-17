using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPopup : MonoBehaviour, IPanelUI
{
	#region Inspector
	[SerializeField] private Button _infoBtn;
	[SerializeField] private Button _confirmBtn;
	[SerializeField] private Button _rotateBtn;
	[SerializeField] private Button _cancelBtn;

    #endregion

    private Transform _target;
    private RectTransform _rt;
    private Camera _camera;

    public Define.UIType Type { get => Define.UIType.EditPopup;}

    private void Awake()
    {
        _confirmBtn.onClick.AddListener(() => 
        {
            BuildingPlacementManager.Instance.PlaceBuilding();
            UIManager.Instance.HideUI(Define.UIType.EditPopup);
        });

        _cancelBtn.onClick.AddListener(() =>
        {
            EditModeController controller = FindObjectOfType<EditModeController>();
            if (controller != null)
            {
                controller.CancelPlacement(); // ✨ 따로 만들 함수 (아래 설명)
            }
            UIManager.Instance.HideUI(Define.UIType.EditPopup);
        });

        _rotateBtn.onClick.AddListener(() =>
        {
            if (_target != null)
            {
                var sr = _target.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.flipX = !sr.flipX; // 좌우 반전 토글
                }
            }
        });

        _rt = GetComponent<RectTransform>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if ( _target == null) return;

        _rt.position = _camera.WorldToScreenPoint(_target.position);
    }

    public void SetTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("SetTarget: target is null");
            return;
        }

        _target = target;
        _rt.position = _camera.WorldToScreenPoint(_target.position);
    }
}
