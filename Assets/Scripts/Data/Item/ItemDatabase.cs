using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
	private static Dictionary<string, ItemData> itemMap = new Dictionary<string, ItemData>();

	public static void LoadJSON()
	{
		TextAsset json = Resources.Load<TextAsset>("Data/item_data");
		var array = JsonUtility.FromJson<ItemArrayWrapper>(json.text);

		foreach (var item in array.items) {
			item.iconSprite = Resources.Load<Sprite>("Data/Icon/"+item.iconPath);
			Debug.Log(item.iconSprite.name);
			itemMap[item.itemID] = item;
		}

		Debug.Log($"아이템 데이터 {itemMap.Count}개 로드 완료");
	}

	public static ItemData Get(string id) => itemMap.TryGetValue(id, out var data) ? data : null;
	public static int GetItemCount() => itemMap.Count;
	public static IEnumerable<ItemData> GetAll() => itemMap.Values;

}