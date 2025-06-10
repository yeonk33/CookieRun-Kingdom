using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { All, Material, Goods, Etc }

public class InventoryUI : MonoBehaviour
{
    public ItemCategory _category = ItemCategory.All;

    private List<string> _curCategoryItems = new List<string>();

    private void OnEnable()
    {
        Inventory.OnChanged -= OnInventoryChanged;
        Inventory.OnChanged += OnInventoryChanged;
    }

    private void OnDisable()
    {
        Inventory.OnChanged -= OnInventoryChanged;
    }

    private void Start()
    {
        SetCategory(ItemCategory.All);
    }

    private void SetCategory(ItemCategory c)
    {
        _category = c;
        Refresh();
    }

    private void Refresh()
    {
        // 데이터 refresh
        var all = Inventory.GetAll();
        _curCategoryItems = ApplyCategory(all);

        // UI refresh
        UpdateUI();
    }

    private void UpdateUI()
    {
        // custom scroll view 사용

    }

    private List<string> ApplyCategory(IEnumerable<string> allItems)
    {
        List<string> result = new List<string>();

        foreach (var itemId in allItems)
        {
            var metaData = ItemDatabase.Get(itemId);
            if (metaData == null) continue;
            if (Inventory.GetCount(itemId) <= 0) continue;
            if (_category == ItemCategory.All || metaData.type == _category.ToString().ToLower())
                result.Add(itemId);
        }

        return result;
    }

    private void OnInventoryChanged(string obj)
    {

    }
}
