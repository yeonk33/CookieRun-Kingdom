using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductionData
{
	public string ProductionId;
	public string displayName;
	public string buildingId;
	public string outputItemId;
	public int outputItemAmout;
	public int coinCost;
	public int timeCost;
	public string iconPath;
	public List<ResourceCost> inputResources;

	[System.NonSerialized]
	public Sprite iconSprite;
}

[System.Serializable]
public class ResourceCost
{
	public string itemId;
	public int amount;
}
