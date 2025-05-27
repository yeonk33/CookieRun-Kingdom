using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProductionPanel : MonoBehaviour
{
	public static ProductionPanel Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	[SerializeField] private TextMeshProUGUI _displayName;
	[SerializeField] private Image _image;

	public void OpenPanel(BuildingData data, int lv)
	{
		if (gameObject.activeSelf) {
			this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		} else {
			gameObject.SetActive(true);
		}

		_displayName.text = data.displayName;
		_image.sprite = data.icon;

		var b = data.buildingLevels.Find(x => x.level == lv);
		List<string> productionsId = b.productions;
		var goodsUI = Resources.Load<GameObject>("Prefabs/GoodsPanel");
		
		// 등록된 생산품만큼 GoodsPanel 추가
		for (int i = 0; i < productionsId.Count; i++) {
			GameObject go = Instantiate(goodsUI, this.transform);
			var production = ProductionDatabase.Get(productionsId[i]);
			var ui = go.GetOrAddComponent<GoodsPanelUI>();

			ui.DisplayName.text = production.displayName;
			ui.GoodsImage.sprite = production.iconSprite;
			ui.SmallImage.sprite = production.iconSprite;
			ui.Amount.text = "X " + production.outputItemAmout.ToString();
			ui.Cost.text = production.coinCost.ToString();
			ui.Time.text = production.timeCost.ToString() + "초";
			
		}
	}
}
