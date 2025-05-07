using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
	public static BuildingManager Instance;

	public GameObject ProducePanel;
	public TextMeshProUGUI BuildingName;
	public Image BuildingIcon;

	public void Awake()
	{
		Instance = this;
		Debug.Log("building manager awake");
	}

	public void OpenPanel()
	{
		if (gameObject.activeSelf) {
			ProducePanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		} else {
			gameObject.SetActive(true);
		}
		// 
		//LoadBuildingJson();
	}

	private void LoadBuildingJson()
	{
		// building json 불러오기

		// BuildingName.text = "나무꾼의 집";
		// BuildingIcon.Sprite = ;
	}
}
