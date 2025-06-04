using System.Collections;
using UnityEngine;

public class ScrollTest : MonoBehaviour
{
	public CustomScrollView ScrollView;     // 에디터에서 연결
	public RectTransform ContentRoot;          // 에디터에서 연결
	public int _itemCount = 500;            // 생성할 아이템 수
	public GameObject _itemPrefab;
	public float ItemWidth;
	public float ItemHeight;

	IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		float viewWidth = ContentRoot.rect.width;
		Debug.Log($"ScrollTest | viewWidth {viewWidth}");
		ScrollView.SlotPrefab = _itemPrefab;
		Debug.Log($"Content height = {ContentRoot.sizeDelta.y}, Viewport height = {ContentRoot.rect.height}");

		RectTransform rectTransform = _itemPrefab.GetComponent<RectTransform>();
		if (rectTransform != null) {
			ItemWidth = rectTransform.rect.width;
			ItemHeight = rectTransform.rect.height;
		}

		// 그리드 레이아웃 생성
		//var layout = new ScrollGrid(ItemWidth, ItemHeight, viewWidth);
		var layout = new ScrollLine(ItemHeight);
		// 스크롤뷰 초기화
		ScrollView.Init(layout, _itemCount, _itemPrefab);
	}
}
