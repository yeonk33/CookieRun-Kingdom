using UnityEngine;

[System.Serializable]
public class ItemData
{
	public string itemID;
	public string displayName;
	public string type;
	public string description;
	public string iconPath;

	[System.NonSerialized]
	public Sprite iconSprite;
}
