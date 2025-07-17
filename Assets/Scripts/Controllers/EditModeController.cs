using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;

public class EditModeController : MonoBehaviour
{
    #region Define
    const string Pivot = "Pivot";
    const string DragAction = "Drag";
    #endregion

    public Tilemap tilemap;

    [SerializeField] InputActionAsset _inputSystem;
    private InputAction _drag;

    private Camera _camera;
    private GameObject _object;
    private Vector3Int _lastCell;
    private Vector3 _offset;
    
    private void Start()
    {
        _camera = Camera.main;
        EventManager.OnBuildingPurchased += HandleOnPurchase;
        BuildingPlacementManager.Instance.OnBuildingPlaced += PlaceBuilding;

        _drag = _inputSystem[DragAction];
        _drag.performed -= OnDragPerformed;
        _drag.performed += OnDragPerformed;
        _drag.started -= OnDragStarted;
        _drag.started += OnDragStarted;
        _drag.canceled -= OnDragCanceled;
        _drag.canceled += OnDragCanceled;
        _drag.Enable();
    }

    private void OnDragStarted(InputAction.CallbackContext context)
    {
        if (!EditState.isEditMode || _object == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = _camera.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        var building = hit.collider?.GetComponent<ProduceBuilding>();

        if (building != null && building.gameObject == _object)
        {
            EditState.IsDraggingBuilding = true;
        }
    }

    private void OnDestroy()
    {
        _drag.performed -= OnDragPerformed;
        _drag.canceled -= OnDragCanceled;
    }
    private void OnDragCanceled(InputAction.CallbackContext context)
    {
        EditState.IsDraggingBuilding = false;
    }

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        if (!EditState.isEditMode || !EditState.IsDraggingBuilding) return;

        Vector3Int cellPos = CalculateCellPos();
        if (cellPos == _lastCell) return;
        _object.transform.position = tilemap.CellToLocal(cellPos) + _offset;

        // 유효성 체크
        TileBase tile = tilemap.GetTile(cellPos); // 나중에 설치 불가능한 타일 추가 될 수 있음 (ex. 물)
        _object.GetComponent<SpriteRenderer>().color = tile != null ? Color.red : Color.white;
    }

    private void SetEditMode()
    {
        _object = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ProductionBuilding"));
        _offset = _object.GetComponentsInChildren<BaseTile>()[0].pivot;
        _object.GetComponent<ProduceBuilding>().SetBuildingData(BuildingDatabase.Get(BuildingId.Smithy), 2);

        Vector3Int cell = CalculateCellPos();
        _object.transform.position = cell;
        _lastCell = cell;

        UIManager.Instance.GetUI(Define.UIType.EditPopup).GetComponent<EditPopup>().SetTarget(_object.transform);
    }


    private void HandleOnPurchase(BuildingData building)
    {
        EditState.isEditMode = true;
        SetEditMode();
        UIManager.Instance.ShowUI(Define.UIType.EditPopup);
    }

    public void PlaceBuilding()
    {
        Vector3Int cell = CalculateCellPos();

        if (!BuildingPlacementManager.Instance.IsCellOccupied(cell)) // 해당 셀에 건물이 없으면
        {
            var produceBuilding = _object.GetComponent<ProduceBuilding>();
            produceBuilding.CellPos = cell;

            BuildingPlacementManager.Instance.RegisterBuilding(produceBuilding);

            BuildingPlacementManager.Instance.SaveAll();
            EditState.isEditMode = false;
            _object = null;
        }
        else
        {
            Debug.Log("이미 건물이 있음!");
        }
    }

    public void CancelPlacement()
    {
        if (_object != null)
        {
            Destroy(_object); // 오브젝트 제거
            _object = null;
        }

        EditState.isEditMode = false;
        EditState.IsDraggingBuilding = false;
    }

    private Vector3Int CalculateCellPos()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(mousePos);
        return cellPos;
    }
}

public static class EditState
{
    public static bool isEditMode = false; // 편집 모드 여부
    public static bool IsDraggingBuilding = false; // 건물 드래그 중 여부
}