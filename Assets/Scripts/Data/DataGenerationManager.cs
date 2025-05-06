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
	private const string _iteScriptPath = "Assets/Scripts/Data/Item/ItemId.cs"; // ��ũ��Ʈ�� ���� ��ũ��Ʈ

	public static void GenerateAll()
	{
		bool itemChanged = Download(_itemSheetName, _itemHashPath, _itemJsonPath, ParseItemCSV);
	}


	/// <summary>
	/// ������ �ٿ�ε� �� ����� ���� �ִٸ� �ؽð� ������Ʈ
	/// </summary>
	private static bool Download(string sheetName, string hashPath, string jsonPath, Func<string, string> parser)
	{
		// google spreadsheet ���
		string url = $"https://docs.google.com/spreadsheets/d/{_sheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";

		try {
			using (WebClient client = new WebClient()) {
				byte[] rowData = client.DownloadData(url); // data �ٿ�
				string csv = Encoding.UTF8.GetString(rowData); // utf-8�� ���ڵ�
				string newHash = GenerateHash(csv);

				bool hashMatch = File.Exists(hashPath) && File.ReadAllText(hashPath) == newHash;
				bool jsonExists = File.Exists(jsonPath);

				if (hashMatch && jsonExists) {
					Debug.Log($"������ ���� ����! {Path.GetFileName(jsonPath)}");
					return false;
				}

				string json = parser(csv);

				File.WriteAllText(hashPath, newHash); // ���ο� �ؽð� �����
				Debug.Log($"{sheetName} �ؽð� ���� �Ϸ�");
				return true;
			}

		} catch (Exception e) {
			Debug.LogError($"������ �ٿ�ε� ���� {sheetName}: {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// csv �������� ���ο� Hash�� �����ϴ� �Լ�
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
			string[] cols = lines[i].Split(','); // csv�� ,�� �и���
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

		return JsonUtility.ToJson(new ItemArrayWrapper { items = fullList }, true); // json ������ return
	}

	// ����� " ����
	private static string Clean(string str) => str.Trim().Trim('"');
}

// Unity�� JsonUtility�� Ŭ������ ����ü�� �ʵ常 ����ȭ ������� ���� ������ ��������.
[System.Serializable]
public class ItemArrayWrapper { public List<ItemData> items; }