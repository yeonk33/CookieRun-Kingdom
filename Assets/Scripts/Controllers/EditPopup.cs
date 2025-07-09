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
        _confirmBtn.onClick.AddListener(() => BuildingPlacementManager.Instance.PlaceBuilding());
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
