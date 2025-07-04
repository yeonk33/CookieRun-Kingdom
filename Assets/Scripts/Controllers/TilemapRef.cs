using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRef : MonoBehaviour
{
    public static TilemapRef Instance;
    public Tilemap tilemap; // 타일맵 직접 할당

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 CellToWorld(Vector3Int cellPos)
    {
        return tilemap.CellToWorld(cellPos);
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }
}
