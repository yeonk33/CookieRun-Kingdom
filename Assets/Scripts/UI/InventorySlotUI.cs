using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IInventorySlot
{
    [SerializeField] private Image _item;
    [SerializeField] private TMP_Text _amountTxt;

    public void SetData(string itemId)
    {
        var item = ProductionDatabase.Get(itemId);
        _item.sprite = Resources.Load<Sprite>($"Data/Icon/{item.iconPath}");
        _amountTxt.text = string.Format("{0:#,###}", Inventory.GetCount(itemId));
    }
}
