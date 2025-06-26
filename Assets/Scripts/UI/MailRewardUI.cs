using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailRewardUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _amount;

    public void SetData(string itemId, int amount)
    {
        var item = ProductionDatabase.Get(itemId);
        _icon.sprite = Resources.Load<Sprite>($"Data/Icon/{item.iconPath}");
        _amount.text = string.Format("{0:#,###}", amount);
    }
}
