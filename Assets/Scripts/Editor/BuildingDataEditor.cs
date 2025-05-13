using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingData))]
public class BuildingDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("자동 생성")) {
			BuildingData data = (BuildingData)target;

			data.buildingLevels = new List<BuildingLevelData>
			{
				new BuildingLevelData
				{
					level = 1,
					buildCost = new List<ResourceCost>
					{
						new ResourceCost { itemId = ItemId.RollCakeWood, amount = 5 },
					},
					productions = FindProductionIdsForBuildingData(data.buildingId),
				},

			};
		}
	}

	/// <summary>
	/// production json 파일에서 param의 buildingId에 해당하는 production id를 List로 가져오기
	/// </summary>
	private List<string> FindProductionIdsForBuildingData(string buildingId)
	{
		TextAsset json = Resources.Load<TextAsset>("Data/production_data");
		var wrapper = JsonUtility.FromJson<ProductionArrayWrapper>(json.text);

		return wrapper.products
			.Where(x=>x.buildingId == buildingId)
			.Select(x=>x.ProductionId)
			.ToList();
	}
}
