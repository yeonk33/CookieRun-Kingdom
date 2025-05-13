using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class ProductionData
{
	[ProductionIdAttribute]
	public string ProductionId;
	public string displayName;
	[BuildingIdAttribute]
	public string buildingId;
	[ItemIdAttribute]
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
	[ItemIdAttribute]
	public string itemId;
	public int amount;
}
