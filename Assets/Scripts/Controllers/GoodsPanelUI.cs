using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoodsPanelUI : MonoBehaviour
{
	[ProductionIdAttribute]
	public string ProductionId;
	[BuildingIdAttribute]
    public string BuildingId;
	public Button ProduceButton;

    [Header("정보 담아야하는 UI")]
	public TMP_Text DisplayName;
	public Image GoodsImage;
	public Image SmallImage;
	public TMP_Text Amount;
	public TMP_Text Cost;
	public TMP_Text Time;

	private ProduceBuilding _building; // 현재 선택된 건물 정보
    private ProductionData _production;

	public void Init(string buildingId, ProduceBuilding building)
	{
		this.BuildingId = buildingId;
        _production = ProductionDatabase.Get(ProductionId);
		Debug.Log($"{ProductionId} {_production.displayName}");
		DisplayName.text = _production.displayName;
		GoodsImage.sprite = _production.iconSprite;
		SmallImage.sprite = _production.iconSprite;
		Amount.text = "X " + _production.outputItemAmout.ToString();
		Cost.text = _production.coinCost.ToString();
		Time.text = _production.timeCost.ToString() + "초";

		_building = building; // 현재 선택된 건물 정보
        ProduceButton.onClick.AddListener(OnProduce);
    }

	public void SetData()
	{

	}

	public void OnProduce()
	{
		//UIController.Instance.ConsumeCoin(Convert.ToInt32(Cost.text));
		//ProductionPanel.Instance.Enqueue(_production);
		ProduceManager.StartProduce(BuildingId, _production.ProductionId, _building);
    }
}
