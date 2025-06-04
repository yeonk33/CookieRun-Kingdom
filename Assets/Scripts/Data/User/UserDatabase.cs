using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserDatabase
{
	private static UserData uidUser = new UserData();

	public static void LoadJSON(int uid)
	{
		TextAsset json = Resources.Load<TextAsset>("Data/user_data");
		var array = JsonUtility.FromJson<UserArrayWrapper>(json.text);

		foreach (var item in array.users) {
			if (item.userId == PlayerPrefs.GetInt("uid")) {
				UserData p = new UserData();
				p.userId = item.userId;
				p.coin = item.coin;
				p.crystal = item.crystal;
				uidUser = p;
				Debug.Log($"{uid} has {p.coin}coin");
				break;
			}
		}
	}

	public static UserData Get(int uid)
	{
		if (uidUser == null) {
			Debug.LogError($"uid:{uid} 유저 찾을 수 없음");
			return null;
		} else return uidUser;
	}

	public static bool AddStock(ItemData goods, int amount)
	{
		if (goods == null || amount == 0) {
			Debug.LogError($"재고 추가 실패");
			return false;
		}



		return true;
	}
}
