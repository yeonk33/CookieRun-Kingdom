using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Game/BuildingData", order = 1)]
public class BuildingData : ScriptableObject
{
	[BuildingIdAttribute]
	public string buildingId;
	public string displayName;
	public Sprite icon;

	public List<BuildingLevelData> buildingLevels;
}

[System.Serializable]
public class BuildingLevelData
{
	public int level;
	public List<ResourceCost> buildCost; // 건설, 업그레이드 비용
	[ProductionIdAttribute]
	public List<string> productions; // 이 건물에서 생산할 수 있는 생산품들
}