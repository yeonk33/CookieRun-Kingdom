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
	private const string _itemScriptPath = "Assets/Scripts/Data/Item/ItemId.cs"; // 스크립트로 만들 스크립트

	private const string _productionSheetName = "Production";
	private const string _productionJsonPath = "Assets/Resources/Data/production_data.json";
	private const string _productionHashPath = "Library/.production_data.hash";
	private const string _productionScriptPath = "Assets/Scripts/Data/Production/ProductionId.cs"; // 스크립트로 만들 스크립트

	public static void GenerateAll()
	{
		bool itemChanged = Download(_itemSheetName, _itemHashPath, _itemJsonPath, ParseItemCSV);
		bool productionChanged = Download(_productionSheetName, _productionHashPath, _productionJsonPath, ParseProductionCSV);

		if (itemChanged) GenerateItemIdClass();
		if (productionChanged) GenerateProductionIdClass();
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

	private static string ParseProductionCSV(string arg)
	{
		List<ProductionData> fullList = new List<ProductionData>();
		string[] lines = arg.Split('\n');

		for (int i = 1; i < lines.Length; i++) {
			string[] cols = lines[i].Split(','); // csv는 ,로 분리됨
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

		return JsonUtility.ToJson(new ProductionArrayWrapper { products = fullList }, true); // json 데이터 return
	}

	public static void GenerateItemIdClass()
	{
		if (!File.Exists(_itemJsonPath)) {
			Debug.LogError($"Json 파일을 찾을 수 없습니다 {_itemJsonPath}");
			return;
		}

		string json = File.ReadAllText(_itemJsonPath);
		ItemArrayWrapper data = JsonUtility.FromJson<ItemArrayWrapper>(json);

		if (data == null || data.items == null) {
			Debug.LogError($"Json 파싱 실패 {_itemJsonPath}");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"/// <summary>");
		sb.AppendLine($"/// 자동 생성된 아이템 Id 상수 목록");
		sb.AppendLine($"/// ItemId.RollCakeWood 로 접근 가능");
		sb.AppendLine($"/// </summary>");
		sb.AppendLine($"public static class ItemId");
		sb.AppendLine("{");

		HashSet<string> keys = new HashSet<string>();
		foreach (var item in data.items) {
			if (string.IsNullOrEmpty(item.itemID)) continue;

			string key = ToPascalCase(item.itemID);
			if (!keys.Add(key)) {
				Debug.LogError($"중복된 키 {key}");
				continue;
			}

			// public const string rollCakeWood = "roll_cake_wood"; 를 작성하는 부분
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
		Debug.Log($"ItemId.cs 생성 완료");
	}

	public static void GenerateProductionIdClass()
	{
		if (!File.Exists(_productionJsonPath)) {
			Debug.LogError($"Json 파일을 찾을 수 없습니다 {_productionJsonPath}");
			return;
		}

		string json = File.ReadAllText(_productionJsonPath);
		ProductionArrayWrapper data = JsonUtility.FromJson<ProductionArrayWrapper>(json);

		if (data == null || data.products == null) {
			Debug.LogError($"Json 파싱 실패 {_productionJsonPath}");
			return;
		}

		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"/// <summary>");
		sb.AppendLine($"/// 자동 생성된 아이템 Id 상수 목록");
		sb.AppendLine($"/// ProductionId.roll_cake_wood_bundle 로 접근 가능");
		sb.AppendLine($"/// </summary>");
		sb.AppendLine($"public static class ProductionId");
		sb.AppendLine("{");

		HashSet<string> keys = new HashSet<string>();
		foreach (var production in data.products) {
			if (string.IsNullOrEmpty(production.ProductionId)) continue;

			string key = ToPascalCase(production.ProductionId);
			if (!keys.Add(key)) {
				Debug.LogError($"중복된 키 {key}");
				continue;
			}

			// public const string rollCakeWood = "roll_cake_wood"; 를 작성하는 부분
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
		Debug.Log($"ProductionId.cs 생성 완료");
	}

	private static string ToPascalCase(string str)
	{
		string[] parts = str.Split('_');
		for (int i = 0; i < parts.Length; i++) {
			parts[i] = char.ToUpperInvariant(parts[i][0]) + parts[i][1..];
		}
		return string.Join("", parts);
	}

	// 공백과 " 제거
	private static string Clean(string str) => str.Trim().Trim('"');
	private static int SafeParseInt(string str, int defaultValue = 0) => int.TryParse(str, out int result) ? result : defaultValue;
}

// Unity의 JsonUtility는 클래스와 구조체의 필드만 직렬화 대상으로 보기 때문에 감싸줘함.
[System.Serializable]
public class ItemArrayWrapper { public List<ItemData> items; }

[System.Serializable]
public class ProductionArrayWrapper { public List<ProductionData> products; }