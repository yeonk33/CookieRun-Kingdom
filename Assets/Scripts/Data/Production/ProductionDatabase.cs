using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProductionDatabase
{
	private static Dictionary<string, ProductionData> productionMap = new Dictionary<string, ProductionData>();

	public static void LoadJSON()
	{
		TextAsset json = Resources.Load<TextAsset>("Data/production_data");
		var array = JsonUtility.FromJson<ProductionArrayWrapper>(json.text);

		foreach (var item in array.products) {
			ProductionData p = new ProductionData();
			p.ProductionId = item.ProductionId;
			p.displayName = item.displayName;
			p.buildingId = item.buildingId;
			p.outputItemId = item.outputItemId;
			p.outputItemAmout = item.outputItemAmout;
			p.coinCost = item.coinCost;
			p.timeCost = item.timeCost;
			p.iconPath = item.iconPath;
			p.inputResources = item.inputResources;
			p.iconSprite = Resources.Load<Sprite>($"Data/Icon/{item.iconPath}");
			productionMap.Add(p.ProductionId, p);
		}
	}

	//public static ProductionData Get(string id) => productionMap.TryGetValue(id, out var data) ? data : null;
	public static ProductionData Get(string id)
	{
		if (productionMap.TryGetValue(id, out var data)) {
			Debug.Log($"{id}, {data.ProductionId}");
			return data;
		} else return null;
	}
	public static int GetCount() => productionMap.Count;
	public static IEnumerable<ProductionData> GetAll() => productionMap.Values;
}
