using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProductionPanel : MonoBehaviour, IPanelUI
{
	public static ProductionPanel Instance { get; private set; }
    public Define.UIType Type => Define.UIType.Produce;

    [SerializeField] private TMP_Text _displayName;
    [SerializeField] private Image _image;
    [SerializeField] private Transform _contentRoot;
	[SerializeField] private List<WaitingSlotUI> _waitings; // 일단 인스펙터에서 연결
    [SerializeField] private GameObject _listBtnRoot;

    private bool _isProducting = false;
    private ProduceBuilding _currentBuilding; // 현재 건물
	private List<GameObject> slots = new();
	private CustomScrollView _scrollView;

    private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);

		Init();
    }


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

	public void Init() // 최초 한 번만
	{
        GameObject goodsUI = Resources.Load<GameObject>("Prefabs/GoodsPanel");

        // 스크롤뷰
        _scrollView = this.GetComponentInChildren<CustomScrollView>();
        _scrollView.SlotPrefab = goodsUI;
        ScrollLine layout = new ScrollLine(goodsUI.GetComponent<RectTransform>().rect.height);
        _scrollView.Init(layout, 4, _scrollView.SlotPrefab);
    }

    private void ProduceManager_OnProduceListUpdated()
    {
        ClearListUI();
        for (int i = 0; i < _currentBuilding.ProduceList.Count; i++)
        {
            float remain = Utils.GetRemainTime(_currentBuilding.ProduceList[i].endTime);
            _waitings[i].SetData(ProductionDatabase.Get(_currentBuilding.ProduceList[i].productionId).iconSprite, remain); // 대기열 UI 업데이트
            //_listImage[i].sprite = ProductionDatabase.Get(_currentBuilding.ProduceList[i].productionId).iconSprite;
            //_listTime[i].text = remain > 0 ? remain.ToString() + "초" : "생산완료";
        }
		if (_currentBuilding.ProduceList.Count != 0 && _currentBuilding.ProduceList.Last<ProduceInfo>().isComplete) _isProducting = false;
		else _isProducting = true; // 생산 중인 상태인지 확인
    }

    public void SetData(BuildingData data, int lv, ProduceBuilding building) // 패널 열 때마다 정보 세팅
	{
		_displayName.text = data.displayName;
		_image.sprite = data.icon;
		_currentBuilding = building; // 현재 선택된 건물 정보

        // 데이터
        var b = data.buildingLevels.Find(x => x.level == lv);
		List<string> productionIds = b.productions;
		_scrollView.SetItemCount(productionIds.Count); // 스크롤뷰 아이템 개수 설정

        // 건물의 생산 가능 레시피만큼 레시피UI 활성화 및 초기화
        for (int i = 0; i < productionIds.Count; i++)
		{
            //ProductionData production = ProductionDatabase.Get(productionIds[i]);
			var ui = _scrollView.Pool[i].GetComponent<GoodsPanelUI>();
            ui.SetData(data.buildingId, building, b.productions[i]);
			ui.gameObject.SetActive(true);
		}

		// 생산 대기열 그리기
		ProduceManager_OnProduceListUpdated();
	}

	private void ClearListUI()
	{
        foreach (var w in _waitings)
        {
            w.EmptySlot();
        }
        //foreach (var img in _listImage) {
        //	img.sprite = null;
        //	img.gameObject.SetActive(false);
        //      }
        //foreach (var txt in _listTime) {
        //	txt.text = null;
        //      }
    }

	private void Update()
	{
		if (!_isProducting) { return; }

		ProduceManager_OnProduceListUpdated();
    }
}
