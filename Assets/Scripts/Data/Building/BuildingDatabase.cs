using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources/BuildingData 안에 있는 BuildingData(Scriptable Object)들 미리 로드해두는 클래스
/// </summary>
public static class BuildingDatabase
{
	private static Dictionary<string, BuildingData> buildingMap = new Dictionary<string, BuildingData>();

	public static void LoadSO()
	{
		var allData = Resources.LoadAll<BuildingData>("BuildingData");
		foreach (var item in allData) {
			buildingMap.Add(item.buildingId, item);
		}
	}

	public static BuildingData Get(string id) => buildingMap.TryGetValue(id, out var data) ? data : null;
	public static int GetItemCount() => buildingMap.Count;
	public static IEnumerable<BuildingData> GetAll() => buildingMap.Values;
}
