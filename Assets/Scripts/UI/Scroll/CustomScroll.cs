using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class CustomScrollView : MonoBehaviour
{
	public GameObject SlotPrefab;		// 재사용 아이템 프리팹
	public RectTransform Viewport;		// 보여지는 영역 (뷰포트)
	public RectTransform ContentRoot;	// 아이템들 담을 영역
	public ScrollRect ScrollRect;       // 스크롤 영역에 붙은 컴포넌트

	public int ItemCount = 2000;       // 전체 개수
	public float ItemWidth;     // 아이템 하나 너비, TODO : 나중에는 prefab에서 가져오도록 개선
	public float ItemHeight;     // 아이템 하나 높이, TODO : 나중에는 prefab에서 가져오도록 개선
	public int BufferCount = 10;       // 추가로 생성할 버퍼 아이템 개수

	private IScrollLayout layout;
	private List<GameObject> _pool = new();	// 재사용 가능한 아이템 풀
	private int _visibleCount = 0;			// 화면에 실제로 보여지는 아이템 개수
	private int _lastFirstIndex = -1;       // 이전 프레임의 시작 인덱스 (같으면 갱산 X)

	public void Init(IScrollLayout layout, int itemCount, GameObject slotPrefab)
	{
		this.layout = layout;
		this.ItemCount = itemCount;
		this.SlotPrefab = slotPrefab;
		this.ContentRoot.sizeDelta = layout.GetContentSize(itemCount);
		this._visibleCount = layout.GetVisibleCount(Viewport.rect.height, BufferCount);
		RectTransform rectTransform = slotPrefab.GetComponent<RectTransform>();
		if (rectTransform != null) {
			this.ItemWidth = rectTransform.rect.width;
			this.ItemHeight = rectTransform.rect.height;
		}

		for (int i = 0; i < _visibleCount; ++i) {
			//GameObject item = Instantiate(SlotPrefab, ContentRoot);
			//var label = item.GetComponentInChildren<TMP_Text>();
			//if (label != null) label.text = $"item_{i}";
			//item.SetActive(false);
			_pool.Add(SlotPrefab);
		}
		

		RefreshItems();
	}

	private void RefreshItems()
	{
		float scrollY = Mathf.Abs(ContentRoot.anchoredPosition.y);
		int firstIndex = layout.GetFirstVisibleIndex(scrollY);

		if (firstIndex == _lastFirstIndex) return; // 보여지고 있는게 똑같으면 refresh X

		_lastFirstIndex = firstIndex;

		// 보이는 개수만 갱신 (= _pool 개수)
		for (int i = 0; i < _pool.Count; i++) {
			int dataIndex = firstIndex + i;

			// 스크롤 범위 밖이면 비활성화
			if (dataIndex < 0 || dataIndex >= ItemCount) {
				_pool[i].gameObject.SetActive(false);
				continue;
			}

			_pool[i].gameObject.SetActive(true);

			// 아이템 위치 LayoutGroup없이 직접 그리기
			_pool[i].transform.localPosition = layout.GetItemPosition(dataIndex);
			//Debug.Log($"[{dataIndex}] Pos = {layout.GetItemPosition(dataIndex)}");

			// TODO: slot prefab마다 다르게 해야하는..?
			//_pool[i].SetData(dataIndex); // 각 slot의 데이터 채워넣기


		}
	}
}
