using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _item;
    [SerializeField] private TMP_Text _amountTxt;

    public void SetUI(string imgPath, int amount)
    {
        _item.sprite = Resources.Load<Sprite>(imgPath);
        _amountTxt.text = string.Format("{0:#,###}", amount);
    }
}
