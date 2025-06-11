using System;
using System.Collections.Generic;
using System.Linq;
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
	[SerializeField] private List<Image> _listImage;
	[SerializeField] private List<TMP_Text> _listTime;
	[SerializeField] private GameObject _listBtnRoot;

    private bool _isProducting = false;
	private ProduceBuilding _currentBuilding; // 현재 건물

	private void OnEnable()
	{
		ProduceManager.OnProduceListUpdated -= ProduceManager_OnProduceListUpdated;
		ProduceManager.OnProduceListUpdated += ProduceManager_OnProduceListUpdated;
		var btns = _listBtnRoot.GetComponentsInChildren<Button>(true);
		foreach (var btn in btns)
		{
			btn.onClick.AddListener(() =>
			{
				ProduceManager.PickupItem(_currentBuilding.InstanceId); // 생산품 수거
            });
		}
	}

    private void OnDisable()
    {
        ProduceManager.OnProduceListUpdated -= ProduceManager_OnProduceListUpdated;
    }

    private void ProduceManager_OnProduceListUpdated()
    {
        ClearListUI();
        for (int i = 0; i < _currentBuilding.ProduceList.Count; i++)
        {
            _listImage[i].sprite = ProductionDatabase.Get(_currentBuilding.ProduceList[i].productionId).iconSprite;
            float remain = Utils.GetRemainTime(_currentBuilding.ProduceList[i].endTime);
            _listTime[i].text = remain > 0 ? remain.ToString() + "초" : "생산완료";
        }
		if (_currentBuilding.ProduceList.Count != 0 && _currentBuilding.ProduceList.Last<ProduceInfo>().isComplete) _isProducting = false;
		else _isProducting = true; // 생산 중인 상태인지 확인
    }

    public void OpenPanel(BuildingData data, int lv, ProduceBuilding building)
	{
		if (gameObject.activeSelf) {
			this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		} else {
			gameObject.SetActive(true);
		}

		_displayName.text = data.displayName;
		_image.sprite = data.icon;
		_currentBuilding = building; // 현재 선택된 건물 정보

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
		}

		// 스크롤뷰
		var scroll = this.GetComponentInChildren<CustomScrollView>();
		var layout = new ScrollLine(goodsUI.GetComponent<RectTransform>().rect.height);
		scroll.Init(layout, productionsId.Count, goodsUI);

		// 생산 대기열 그리기
		ProduceManager_OnProduceListUpdated();
    }

	private void ClearListUI()
	{
		foreach (var img in _listImage) {
			img.sprite = null;
		}
		foreach (var txt in _listTime) {
			txt.text = null;
        }
    }

	private void Update()
	{
		if (!_isProducting) { return; }

		ProduceManager_OnProduceListUpdated();
    }
}
