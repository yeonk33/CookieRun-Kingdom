using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EditModeController : MonoBehaviour
{
    #region Define
    const string Pivot = "Pivot";
    #endregion

    private Camera _camera;
    private GameObject _object;
    private Vector3Int _lastCell;
    private Vector3 _offset;
    private BuildingShopController _buildingShopController;
    private GameObject _editPopup;

    public Tilemap tilemap;
    public bool isEditMode;

    private void Start()
    {
        _camera = Camera.main;
        EventManager.OnBuildingPurchased += HandleOnPurchase;
        BuildingPlacementManager.Instance.OnBuildingPlaced += PlaceBuilding;
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
        //if (_editPopup == null)
        //{
        //    Debug.Log("ee");
        //    _editPopup = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/EditPopup"));
        //}
        //Debug.Log("ff");
        //_editPopup.SetActive(true);
    }


    private void HandleOnPurchase(BuildingData building)
    {
        isEditMode = true;
        SetEditMode();
        UIManager.Instance.ShowUI(Define.UIType.EditPopup);
    }

    private void Update()
    {
        if (!isEditMode) return;

        if (CalculateCellPos() == _lastCell || !isEditMode) return;
        Vector3Int cellPos = CalculateCellPos();
        _object.transform.position = tilemap.CellToLocal(cellPos) + _offset;

        TileBase tile = tilemap.GetTile(cellPos); // 나중에 설치 불가능한 타일 추가 될 수 있음 (ex. 물)
        if (tile != null)
        {
            _object.GetComponent<SpriteRenderer>().color = Color.red;
            return;
        }
        else
        {
            _object.GetComponent<SpriteRenderer>().color = Color.white;
        }

        //if (Input.GetKeyUp(KeyCode.Space)) // 설치
        //{
        //    PlaceBuilding();
        //}
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
            isEditMode = false;
            _object = null;
        }
        else
        {
            Debug.Log("이미 건물이 있음!");
        }
    }

    private Vector3Int CalculateCellPos()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(mousePos);
        return cellPos;
    }
}
