using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuildingSaveData
{
    public string buildingId;
    public string instanceId;
    public int level;
    public Vector3Int cellPos;
}

public class BuildingPlacementManager : MonoBehaviour
{
    public static BuildingPlacementManager Instance;

    private List<ProduceBuilding> _activeBuildings = new();
    public event Action OnBuildingPlaced;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterBuilding(ProduceBuilding building)
    {
        _activeBuildings.Add(building);
    }

    public bool IsCellOccupied(Vector3Int cell)
    {
        return _activeBuildings.Any(b => b.CellPos == cell);
    }

    public void SaveAll()
    {
        var saveList = new List<BuildingSaveData>();

        foreach (var b in _activeBuildings)
        {
            saveList.Add(new BuildingSaveData
            {
                buildingId = b.BuildingId, // Or b.GetData().buildingId
                instanceId = b.InstanceId,
                level = b.Level,
                cellPos = b.CellPos
            });
        }

        var json = JsonUtility.ToJson(new SerializationWrapper<BuildingSaveData> { list = saveList }, true);
        System.IO.File.WriteAllText("Assets/Resources/Data/building_location.json", json);
        Debug.Log("Building data saved successfully.");
    }

    public void LoadAll()
    {
        string path = "Assets/Resources/Data/building_location.json";
        if (!System.IO.File.Exists(path)) return;

        var json = System.IO.File.ReadAllText(path);
        var data = JsonUtility.FromJson<SerializationWrapper<BuildingSaveData>>(json);

        foreach (var d in data.list)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/ProductionBuilding");
            var go = Instantiate(prefab);
            var pb = go.GetComponent<ProduceBuilding>();
            var buildingData = BuildingDatabase.Get(d.buildingId);

            pb.SetBuildingData(buildingData, d.level, d.instanceId);
            pb.CellPos = d.cellPos;
            pb.transform.position = TilemapRef.Instance.CellToWorld(d.cellPos); // 타일맵 참조 필요
            RegisterBuilding(pb);
        }
    }

    public void PlaceBuilding()
    {
        OnBuildingPlaced?.Invoke();
    }

    [System.Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> list;
    }
}
