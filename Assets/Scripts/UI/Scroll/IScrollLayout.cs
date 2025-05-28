using UnityEngine;

public interface IScrollLayout
{
	Vector2 GetItemPosition(int index);
	Vector2 GetContentSize(int itemCount);
	int GetFirstVisibleIndex(float scrollY);
	int GetVisibleCount(float viewHeight, int buffCount);
}
