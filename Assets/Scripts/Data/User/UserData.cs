using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
	public int userId;
	public int coin;
	public int crystal;
	public List<StockInfo> stocks;
}

[System.Serializable]
public class StockInfo
{
	[ItemIdAttribute]
	public string itemId;
	public int amount;
}
