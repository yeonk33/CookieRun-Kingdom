using UnityEngine;

public class ScrollLine : IScrollLayout
{
	private float _itemHeight;

	public ScrollLine(float h)
	{
		this._itemHeight = h;
	}

	public Vector2 GetItemPosition(int index)
	{
		return new Vector2(0, -index * _itemHeight);
	}

	public Vector2 GetContentSize(int itemCount)
	{
		return new Vector2(0, itemCount * _itemHeight);
	}

	public int GetFirstVisibleIndex(float scrollY)
	{
		return Mathf.FloorToInt(scrollY / _itemHeight);
	}

	public int GetVisibleCount(float viewHeight, int buffCount)
	{
		return Mathf.CeilToInt(viewHeight / _itemHeight) + buffCount;
	}
}
