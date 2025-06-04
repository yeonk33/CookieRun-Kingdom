using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class GoodsPanelUI : MonoBehaviour
{
	[ProductionIdAttribute]
	public string ProductionId;

	[Header("정보 담아야하는 UI")]
	public TextMeshProUGUI DisplayName;
	public Image GoodsImage;
	public Image SmallImage;
	public TextMeshProUGUI Amount;
	public TextMeshProUGUI Cost;
	public TextMeshProUGUI Time;

	private void OnEnable()
	{
		var production = ProductionDatabase.Get(ProductionId);
		Debug.Log($"{ProductionId} {production.displayName}");
		DisplayName.text = production.displayName;
		GoodsImage.sprite = production.iconSprite;
		SmallImage.sprite = production.iconSprite;
		Amount.text = "X " + production.outputItemAmout.ToString();
		Cost.text = production.coinCost.ToString();
		Time.text = production.timeCost.ToString() + "초";
	}

	public void SetData()
	{

	}
}
