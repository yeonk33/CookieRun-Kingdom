using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingOnTilemap : MonoBehaviour
{
    public Tilemap building;
    private void OnMouseUp()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3Int cellPos = building.WorldToCell(mousePos);
        Debug.Log($"{building.GetTile(cellPos).name} building click");
    }
}
