using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DataGenerationManager
{
	private const string _sheetId = "1xJBhZHkJOecj9tKmt8FIbkzkCmjhDdv35cuZTS3mrwA";
	private const string _itemSheetName = "Item";
	private const string _itemJsonPath = "Assets/Resources/Data/item_data.json";
	private const string _itemHashPath = "Library/.item_data.hash";
	private const string _iteScriptPath = "Assets/Scripts/Data/Item/ItemId.cs"; // 스크립트로 만들 스크립트

	public static void GenerateAll()
	{
		bool itemChanged = Download(_itemSheetName, _itemHashPath, _itemJsonPath, ParseItemCSV);
	}


	/// <summary>
	/// 데이터 다운로드 후 변경된 것이 있다면 해시값 업데이트
	/// </summary>
	private static bool Download(string sheetName, string hashPath, string jsonPath, Func<string, string> parser)
	{
		// google spreadsheet 사용
		string url = $"https://docs.google.com/spreadsheets/d/{_sheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

		try {
			using (WebClient client = new WebClient()) {
				byte[] rowData = client.DownloadData(url); // data 다운
				string csv = Encoding.UTF8.GetString(rowData); // utf-8로 인코딩
				string newHash = GenerateHash(csv);

				bool hashMatch = File.Exists(hashPath) && File.ReadAllText(hashPath) == newHash;
				bool jsonExists = File.Exists(jsonPath);

				if (hashMatch && jsonExists) {
					Debug.Log($"데이터 변경 없음! {Path.GetFileName(jsonPath)}");
					return false;
				}

				string json = parser(csv);

				File.WriteAllText(hashPath, newHash); // 새로운 해시값 덮어쓰기
				Debug.Log($"{sheetName} 해시값 변경 완료");
				return true;
			}

		} catch (Exception e) {
			Debug.LogError($"데이터 다운로드 실패 {sheetName}: {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// csv 내용으로 새로운 Hash값 생성하는 함수
	/// </summary>
	private static string GenerateHash(string csv)
	{
		using (var sha = SHA256.Create()) {
			byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(csv));
			return Convert.ToBase64String(hash);
		}
	}

	private static string ParseItemCSV(string arg)
	{
		List<ItemData> fullList = new List<ItemData>();
		string[] lines = arg.Split('\n');

		for (int i = 1; i < lines.Length; i++) {
			string[] cols = lines[i].Split(','); // csv는 ,로 분리됨
			ItemData fullData = new ItemData
			{
				itemID = Clean(cols[0]),
				displayName = Clean(cols[1]),
				type = Clean(cols[2]),
				description = Clean(cols[3]),
				iconPath = Clean(cols[4])
			};
			fullList.Add(fullData);

		}

		File.WriteAllText(_itemJsonPath, JsonUtility.ToJson(new ItemArrayWrapper { items = fullList }, true), new UTF8Encoding(true));

		return JsonUtility.ToJson(new ItemArrayWrapper { items = fullList }, true); // json 데이터 return
	}

	// 공백과 " 제거
	private static string Clean(string str) => str.Trim().Trim('"');
}

// Unity의 JsonUtility는 클래스와 구조체의 필드만 직렬화 대상으로 보기 때문에 감싸줘함.
[System.Serializable]
public class ItemArrayWrapper { public List<ItemData> items; }