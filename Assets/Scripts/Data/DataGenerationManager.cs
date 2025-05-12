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
	private const string _itemScriptPath = "Assets/Scripts/Data/Item/ItemId.cs"; // ��ũ��Ʈ�� ���� ��ũ��Ʈ

	private const string _productionSheetName = "Production";
	private const string _productionJsonPath = "Assets/Resources/Data/production_data.json";
	private const string _productionHashPath = "Library/.production_data.hash";
	private const string _productionScriptPath = "Assets/Scripts/Data/Production/ProductionId.cs"; // ��ũ��Ʈ�� ���� ��ũ��Ʈ

	public static void GenerateAll()
	{
		bool itemChanged = Download(_itemSheetName, _itemHashPath, _itemJsonPath, ParseItemCSV);
		bool productionChanged = Download(_productionSheetName, _productionHashPath, _productionJsonPath, ParseProductionCSV);

		if (itemChanged) GenerateItemIdClass();
		if (productionChanged) GenerateProductionIdClass();
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

	private static string ParseProductionCSV(string arg)
	{
		List<ProductionData> fullList = new List<ProductionData>();
		string[] lines = arg.Split('\n');

		for (int i = 1; i < lines.Length; i++) {
			string[] cols = lines[i].Split(','); // csv�� ,�� �и���
			ProductionData fullData = new ProductionData
			{
				ProductionId = Clean(cols[0]),
				displayName = Clean(cols[1]),
				buildingId = Clean(cols[2]),
				outputItemId = Clean(cols[3]),
				outputItemAmout = SafeParseInt(Clean(cols[4])),
				coinCost = SafeParseInt(Clean(cols[5])),
				timeCost = SafeParseInt(Clean(cols[6])),
				iconPath = Clean(cols[7]),
				inputResources = new List<ResourceCost>(),

			};

			if (!string.IsNullOrWhiteSpace(Clean(cols[8]))) {
				fullData.inputResources.Add(new ResourceCost { itemId = Clean(cols[8]), amount = SafeParseInt(cols[9])});
			}
			if (!string.IsNullOrWhiteSpace(Clean(cols[10]))) {
				fullData.inputResources.Add(new ResourceCost { itemId = Clean(cols[10]), amount = SafeParseInt(cols[11]) });
			}

			fullList.Add(fullData);

		}

		File.WriteAllText(_productionJsonPath, JsonUtility.ToJson(new ProductionArrayWrapper { products = fullList }, true), new UTF8Encoding(true));

		return JsonUtility.ToJson(new ProductionArrayWrapper { products = fullList }, true); // json ������ return
	}

	public static void GenerateItemIdClass()
	{
		if (!File.Exists(_itemJsonPath)) {
			Debug.LogError($"Json ������ ã�� �� �����ϴ� {_itemJsonPath}");
			return;
		}

		string json = File.ReadAllText(_itemJsonPath);
		ItemArrayWrapper data = JsonUtility.FromJson<ItemArrayWrapper>(json);

		if (data == null || data.items == null) {
			Debug.LogError($"Json �Ľ� ���� {_itemJsonPath}");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"/// <summary>");
		sb.AppendLine($"/// �ڵ� ������ ������ Id ��� ���");
		sb.AppendLine($"/// ItemId.RollCakeWood �� ���� ����");
		sb.AppendLine($"/// </summary>");
		sb.AppendLine($"public static class ItemId");
		sb.AppendLine("{");

		HashSet<string> keys = new HashSet<string>();
		foreach (var item in data.items) {
			if (string.IsNullOrEmpty(item.itemID)) continue;

			string key = ToPascalCase(item.itemID);
			if (!keys.Add(key)) {
				Debug.LogError($"�ߺ��� Ű {key}");
				continue;
			}

			// public const string rollCakeWood = "roll_cake_wood"; �� �ۼ��ϴ� �κ�
			sb.AppendLine($"	public const string {key} = \"{item.itemID}\";");
		}

		sb.AppendLine();
		sb.AppendLine("\tpublic static readonly string[] All = new[]");
		sb.AppendLine("\t{");

		foreach (var item in data.items) {
			string key = ToPascalCase(item.itemID);
			sb.AppendLine($"\t\t{key},");
		}
		sb.AppendLine("\t};");
		sb.AppendLine("}");
		
		File.WriteAllText(_itemScriptPath, sb.ToString(), Encoding.UTF8);
		Debug.Log($"ItemId.cs ���� �Ϸ�");
	}

	public static void GenerateProductionIdClass()
	{
		if (!File.Exists(_productionJsonPath)) {
			Debug.LogError($"Json ������ ã�� �� �����ϴ� {_productionJsonPath}");
			return;
		}

		string json = File.ReadAllText(_productionJsonPath);
		ProductionArrayWrapper data = JsonUtility.FromJson<ProductionArrayWrapper>(json);

		if (data == null || data.products == null) {
			Debug.LogError($"Json �Ľ� ���� {_productionJsonPath}");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"/// <summary>");
		sb.AppendLine($"/// �ڵ� ������ ������ Id ��� ���");
		sb.AppendLine($"/// ProductionId.roll_cake_wood_bundle �� ���� ����");
		sb.AppendLine($"/// </summary>");
		sb.AppendLine($"public static class ProductionId");
		sb.AppendLine("{");

		HashSet<string> keys = new HashSet<string>();
		foreach (var production in data.products) {
			if (string.IsNullOrEmpty(production.ProductionId)) continue;

			string key = ToPascalCase(production.ProductionId);
			if (!keys.Add(key)) {
				Debug.LogError($"�ߺ��� Ű {key}");
				continue;
			}

			// public const string rollCakeWood = "roll_cake_wood"; �� �ۼ��ϴ� �κ�
			sb.AppendLine($"	public const string {key} = \"{production.ProductionId}\";");
		}

		sb.AppendLine();
		sb.AppendLine("\tpublic static readonly string[] All = new[]");
		sb.AppendLine("\t{");

		foreach (var production in data.products) {
			string key = ToPascalCase(production.ProductionId);
			sb.AppendLine($"\t\t{key},");
		}
		sb.AppendLine("\t};");
		sb.AppendLine("}");

		File.WriteAllText(_productionScriptPath, sb.ToString(), Encoding.UTF8);
		Debug.Log($"ProductionId.cs ���� �Ϸ�");
	}

	private static string ToPascalCase(string str)
	{
		string[] parts = str.Split('_');
		for (int i = 0; i < parts.Length; i++) {
			parts[i] = char.ToUpperInvariant(parts[i][0]) + parts[i][1..];
		}
		return string.Join("", parts);
	}

	// ����� " ����
	private static string Clean(string str) => str.Trim().Trim('"');
	private static int SafeParseInt(string str, int defaultValue = 0) => int.TryParse(str, out int result) ? result : defaultValue;
}

// Unity�� JsonUtility�� Ŭ������ ����ü�� �ʵ常 ����ȭ ������� ���� ������ ��������.
[System.Serializable]
public class ItemArrayWrapper { public List<ItemData> items; }

[System.Serializable]
public class ProductionArrayWrapper { public List<ProductionData> products; }