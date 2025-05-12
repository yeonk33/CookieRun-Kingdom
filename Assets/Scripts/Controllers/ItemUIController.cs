using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemUIController : MonoBehaviour
{
	public Transform itemPanel; // 아이템을 넣을 UI Panel

	private void Start()
	{
		foreach (var item in ItemDatabase.GetAll()) {
			if (item.iconSprite == null) continue;

			GameObject go = new GameObject("ItemImage", typeof(RectTransform), typeof(Image));
			go.transform.SetParent(itemPanel, false); // 패널 아래에 붙이기

			Image image = go.GetComponent<Image>();
			image.sprite = item.iconSprite; // 이미지 설정
		}
	}
}
