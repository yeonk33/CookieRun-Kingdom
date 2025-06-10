using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
	public int userId;
	public int coin;
	public int crystal;
	public List<StockInfo> stocks;
	public List<ProduceBuilding> buildings; // 배치된 생산 건물들

}

//public class BuildingInfo
//{
//	public string buildingId;
//}

[System.Serializable]
public class StockInfo
{
	[ItemIdAttribute]
	public string itemId;
	public int amount;
}
