using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { All, Material, Goods, Etc }

public class InventoryUI : MonoBehaviour
{
    public ItemCategory _category = ItemCategory.All;

    public CustomScrollView ScrollView;     // 에디터에서 연결
    private List<string> _curCategoryItems = new List<string>();


    private void OnEnable()
    {
        Inventory.OnChanged -= OnInventoryChanged;
        Inventory.OnChanged += OnInventoryChanged;
        Refresh();
    }

    private void OnDisable()
    {
        Inventory.OnChanged -= OnInventoryChanged;
    }

    private void Awake()
    {
        var slot = Resources.Load<GameObject>("Prefabs/Inventory Slot");
        var size = slot.GetComponent<RectTransform>();
        //ScrollView.Init(new ScrollGrid(size.rect.size.x, size.rect.size.y, ScrollView.Viewport.rect.width), Inventory.GetTotalCount(), slot);
        ScrollView.Init(new ScrollGrid(size.rect.size.x, size.rect.size.y, ScrollView.Viewport.rect.width), 400, slot); // 테스트용
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
        OnInventoryChanged("");
    }

    private List<string> ApplyCategory(IEnumerable<string> allItems)
    {
        List<string> result = new List<string>(); // 현재 카테고리에 해당하는 itemId 담을 리스트

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
        ScrollView.RefreshItems(); // Slot 다시 그리기

        for (int i = 0; i < _curCategoryItems.Count; ++i)
        {
            var itemID = _curCategoryItems[i];

            IInventorySlot slot = null;
            if (i < ScrollView.VisibleCount)
                slot = ScrollView.Pool[i].GetComponent<IInventorySlot>();  // 재활용
            else
                Debug.LogWarning("Not enough slots in the pool!"); // 풀에 아이템이 부족할 경우 경고
                //slot = Instantiate(, _slotRoot).GetComponent<IInventorySlot>();

            slot.SetData(itemID);

            //_itemSlots[itemID] = slot;
        }

        //for (int i = _curCategoryItems.Count; i < _slotRoot.childCount; ++i)
        //{
        //    _slotRoot.GetChild(i).gameObject.SetActive(false);  // 재활용 안한 애들 비활성화
        //}
    }
}
