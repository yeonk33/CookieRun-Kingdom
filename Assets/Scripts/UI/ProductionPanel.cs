using System;
using System.Collections.Generic;
using System.Threading;
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

	[SerializeField] private TMP_Text _displayName;
	[SerializeField] private Image _image;
	[SerializeField] private Transform _contentRoot;
	[SerializeField] private Image _listImage; // @@@@@ 나중에 LIst<>에 담기
	[SerializeField] private TMP_Text _listTime;// @@@@@ 나중에 LIst<>에 담기
	
	private bool _isProducting = false;
	private DateTime _endTime;
	private string _productingId;
	private int _productingCount;

	public void OpenPanel(BuildingData data, int lv, ProduceBuilding building)
	{
		if (gameObject.activeSelf) {
			this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		} else {
			gameObject.SetActive(true);
		}

		_displayName.text = data.displayName;
		_image.sprite = data.icon;

		// 데이터
		var b = data.buildingLevels.Find(x => x.level == lv);
		List<string> productionsId = b.productions;
		var goodsUI = Resources.Load<GameObject>("Prefabs/GoodsPanel");

		// 등록된 생산품만큼 GoodsPanel 추가
		for (int i = 0; i < productionsId.Count; i++) {
			GameObject go = Instantiate(goodsUI, _contentRoot);
			var production = ProductionDatabase.Get(productionsId[i]);
			var ui = go.GetOrAddComponent<GoodsPanelUI>();
			ui.Init(data.buildingId, building);

            ui.ProductionId = b.productions[i];
			go.SetActive(true);
			//ui.DisplayName.text = production.displayName;
			//ui.GoodsImage.sprite = production.iconSprite;
			//ui.SmallImage.sprite = production.iconSprite;
			//ui.Amount.text = "X " + production.outputItemAmout.ToString();
			//ui.Cost.text = production.coinCost.ToString();
			//ui.Time.text = production.timeCost.ToString() + "초";
			
		}

		// 스크롤뷰
		var scroll = this.GetComponentInChildren<CustomScrollView>();
		var layout = new ScrollLine(goodsUI.GetComponent<RectTransform>().rect.height);
		scroll.Init(layout, productionsId.Count, goodsUI);
	}

	public void Enqueue(ProductionData production)
	{
		if (_isProducting) { Debug.Log("생산대기열이 꽉찼습니다."); return; } // @@@@ text로 화면에 띄우기
		_listImage.sprite = production.iconSprite;
		_listTime.text = production.timeCost.ToString() + "초";
		_endTime = DateTime.UtcNow.AddSeconds(production.timeCost);
        _productingId = production.ProductionId;
        _productingCount = production.outputItemAmout;
        Debug.Log($"{production.displayName} {_endTime}에 생산 완료.");
		PlayerPrefs.SetString("endTime", _endTime.ToString());
		PlayerPrefs.Save();
		_isProducting = true;
	}

	private void Update()
	{
		if (!_isProducting) { return; }

		var remainTime = _endTime - DateTime.UtcNow;
		if (remainTime.TotalSeconds > 0) {
			_listTime.text = Mathf.CeilToInt((float)remainTime.TotalSeconds).ToString() + "초";
		} else {
			Debug.Log("생산 완료");
			 Inventory.Add(_productingId, _productingCount); // @@@@@@ 추가예정
			_listImage.sprite = null;
			_listTime.text = null;
			_isProducting = false;
		}
	}
}
