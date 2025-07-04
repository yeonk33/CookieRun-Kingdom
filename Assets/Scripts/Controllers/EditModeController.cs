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

    public Tilemap tilemap;
    public bool isEditMode;


    private void Start()
    {
        _camera = Camera.main;
        EventManager.OnBuildingPurchased += HandleOnPurchase;
    }

    private void SetEditMode()
    {
        _object = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ProductionBuilding"));
        _offset = _object.GetComponentsInChildren<BaseTile>()[0].pivot;
        _object.GetComponent<ProduceBuilding>().SetBuildingData(BuildingDatabase.Get(BuildingId.Smithy), 2);

        Vector3Int cell = CalculateCellPos();
        _object.transform.position = cell;
        _lastCell = cell;
    }


    private void HandleOnPurchase(BuildingData building)
    {
        isEditMode = true;
        SetEditMode();
    }

    private void Update()
    {
        if (!isEditMode) return;

        if (Input.GetKeyUp(KeyCode.Space)) // 설치
        {
            Debug.Log("건물 설치!");
            isEditMode = false;
            _object = null;
        }

        if (CalculateCellPos() == _lastCell || !isEditMode) return;
        Vector3Int cellPos = CalculateCellPos();
        _object.transform.position = tilemap.CellToLocal(cellPos) + _offset;

        TileBase tile = tilemap.GetTile(cellPos);
        if (tile != null)
        {
            _object.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            _object.GetComponent<SpriteRenderer>().color = Color.white;
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
