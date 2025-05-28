using System.Collections;
using UnityEngine;

public class ScrollTest : MonoBehaviour
{
	public CustomScrollView ScrollView;     // 에디터에서 연결
	public RectTransform ContentRoot;          // 에디터에서 연결
	public int _itemCount = 500;            // 생성할 아이템 수
	public float ItemWidth = 90f;
	public float ItemHeight = 90f;
	public GameObject _itemPrefab;

	IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		float viewWidth = ContentRoot.rect.width;
		Debug.Log($"ScrollTest | viewWidth {viewWidth}");
		ScrollView.SlotPrefab = _itemPrefab;
		Debug.Log($"Content height = {ContentRoot.sizeDelta.y}, Viewport height = {ContentRoot.rect.height}");

		// 그리드 레이아웃 생성
		var layout = new ScrollGrid(ItemWidth, ItemHeight, viewWidth);
		// 스크롤뷰 초기화
		ScrollView.Init(layout, _itemCount, _itemPrefab);
	}
}
