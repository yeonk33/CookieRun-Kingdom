using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsPanelUI : MonoBehaviour
{
	[ProductionIdAttribute]
	public string ProductionId;

	[Header("정보 담아야하는 UI")]
	public TMP_Text DisplayName;
	public Image GoodsImage;
	public Image SmallImage;
	public TMP_Text Amount;
	public TMP_Text Cost;
	public TMP_Text Time;

	private ProductionData _production;

	private void OnEnable()
	{
		_production = ProductionDatabase.Get(ProductionId);
		Debug.Log($"{ProductionId} {_production.displayName}");
		DisplayName.text = _production.displayName;
		GoodsImage.sprite = _production.iconSprite;
		SmallImage.sprite = _production.iconSprite;
		Amount.text = "X " + _production.outputItemAmout.ToString();
		Cost.text = _production.coinCost.ToString();
		Time.text = _production.timeCost.ToString() + "초";
	}

	public void SetData()
	{

	}

	public void OnProduce()
	{
		UIController.Instance.ConsumeCoin(Convert.ToInt32(Cost.text));
		ProductionPanel.Instance.Enqueue(_production);
	}
}
