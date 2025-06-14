using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* * CustomScrollView.cs
 * 
 * ScrollView에 각 Slot이 어떤 형태 (Line, Grid)로 그려지는지 정의하고, 
 * Viewport 사이즈에 들어갈 개수만큼만 그리는 클래스
 */

public class CustomScrollView : MonoBehaviour
{
	public GameObject SlotPrefab;		// 재사용 아이템 프리팹
	public RectTransform Viewport;		// 보여지는 영역 (뷰포트)
	public RectTransform ContentRoot;	// 아이템들 담을 영역
	public ScrollRect ScrollRect;       // 스크롤 영역에 붙은 컴포넌트

	public int ItemCount = 2000;		// 전체 개수
	public float ItemWidth;				// 아이템 하나 너비
	public float ItemHeight;			// 아이템 하나 높이
	public int BufferCount = 10;		// 추가로 생성할 버퍼 아이템 개수

	private IScrollLayout layout;
	private List<GameObject> _pool = new();	// 재사용 가능한 아이템 풀
	private int _visibleCount = 0;			// 화면에 실제로 보여지는 아이템 개수
	private int _lastFirstIndex = -1;       // 이전 프레임의 시작 인덱스 (같으면 갱산 X)

	public int VisibleCount => _visibleCount;	// 화면에 실제로 보여지는 아이템 개수
	public List<GameObject> Pool => _pool;		// 재사용 가능한 아이템 풀

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
            var go = Instantiate(SlotPrefab, ContentRoot);
            _pool.Add(go);
		}

        // 스크롤시 아이템 갱신
        ScrollRect.onValueChanged.AddListener(_ => RefreshItems());

        RefreshItems();
	}

	public void RefreshItems()
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
        }
	}
}
