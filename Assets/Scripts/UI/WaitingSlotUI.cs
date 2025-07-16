using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingSlotUI : MonoBehaviour
{
    [SerializeField] private Image _item;
    [SerializeField] private GameObject _time;
    [SerializeField] private TMP_Text _timeText;

    public void EmptySlot()
    {
        _item.gameObject.SetActive(false);
        _time.SetActive(false);
    }

    public void SetData(Sprite item, float time)
    {
        _item.sprite = item;
        _item.gameObject.SetActive(true);
        _time.SetActive(true);

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeTxt = "";
        timeTxt += timeSpan.Hours != 0 ? $"{timeSpan.Hours}시" : "";
        timeTxt += timeSpan.Minutes != 0 ? $"{timeSpan.Minutes}분" : "";
        timeTxt += timeSpan.Seconds != 0 ? $"{timeSpan.Seconds}초" : "생산완료";

        _timeText.text = timeTxt;
    }
}
