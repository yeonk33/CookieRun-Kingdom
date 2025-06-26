using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailboxSlotUI : MonoBehaviour, IScrollSlot
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private Transform _rewardRoot;
    [SerializeField] private List<MailRewardUI> _rewards;

    public void SetDataWithData(string message, object data)
    {
        _message.text = message;
        _icon.sprite = Resources.Load<Sprite>($"Data/Icon/MailIcon");
        _rewards.Clear();
        _rewards = data as List<MailRewardUI>;

        foreach (var reward in _rewards)
        {
            var rewardInstance = Instantiate(reward, _rewardRoot);
        }
    }

    #region IScrollSlot Implementation
    public void SetData(string id)
    {
        
    }
    #endregion
}

