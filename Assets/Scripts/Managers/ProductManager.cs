using System.Collections.Generic;

// 일괄 수거 등을 위한 중앙매니저 관리
public static class ProduceManager
{
    // 생산 중인 건물만 담기 <instanceId, ProduceBuilding>
    private static Dictionary<string, ProduceBuilding> _buildings = new Dictionary<string, ProduceBuilding>(); 

    public static ProduceBuilding GetBuilding(string instanceId)
    {
        if (_buildings.ContainsKey(instanceId))
        {
            return _buildings[instanceId];
        }
        return null;
    }
   
    public static Dictionary<string, ProduceBuilding> GetAllBuildings() => _buildings;

    public static void RegistBuilding(string instanceId, ProduceBuilding building)
    {
        if (!_buildings.ContainsKey(instanceId))
        {
            _buildings.Add(instanceId, building);
        }
    }

    public static void UnregistBuilding(string buildingId)
    {
        if (_buildings.ContainsKey(buildingId))
        {
            _buildings.Remove(buildingId);
        }
    }

    public static void StartProduce(string instanceId, string productionId, ProduceBuilding building) // 생산 시작 (버튼에 연결)
    {
        RegistBuilding(instanceId, building);
        building.StartProduce(productionId); // 건물 생산 대기열 등록
    }

    public static void PickupItem(string instanceId) // 생산품 수거
    {
        if (!_buildings.ContainsKey(instanceId)) return;

        var building = _buildings[instanceId];
        if (building.ProduceList.Count == 0) return;
        building.PickupItems();

        if (building.ProduceList.Count == 0) UnregistBuilding(instanceId);
    }

    public static void PickupAllItems() // 모든 생산품 수거
    {
        foreach (var building in _buildings.Values)
        {
            building.PickupItems();
        }
        
        // 수거 후 비우기
        //_buildings.Clear();
    }
}
