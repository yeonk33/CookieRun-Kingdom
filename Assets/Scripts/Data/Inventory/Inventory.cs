using System.Collections.Generic;

public static class Inventory
{
    private static Dictionary<string, int> _items = new Dictionary<string, int>();

    public static event System.Action<string> OnChanged; // UI에 알림을 위한 이벤트

    public static void Add(string itemId, int amount)
    {
        if (!_items.ContainsKey(itemId)) {
            _items[itemId] = 0;
        }

        _items[itemId] += amount;
        OnChanged?.Invoke(itemId); // UI에 알림

        DebugInventory(); // 디버그용
    }

    public static bool HasEnough(string itemId, int required)
    {
        return GetCount(itemId) >= required;
    }

    public static void Consume(string itemId, int amount = 1)
    {
        if (!HasEnough(itemId, amount)) return;
        _items[itemId] -= amount;
        OnChanged?.Invoke(itemId); // UI에 알림
    }

    /// <summary>
    /// itemId 개수 반환
    /// </summary>
    public static int GetCount(string itemId)
    {
        return _items.TryGetValue(itemId, out int count) ? count : 0;
    }

    public static void DebugInventory()
    {
        foreach (var item in _items)
        {
            UnityEngine.Debug.Log($"Item: {item.Key}, Count: {item.Value}");
        }
    }

    public static IEnumerable<string> GetAll() => _items.Keys;
    public static int GetTotalCount() => _items.Count;
}
