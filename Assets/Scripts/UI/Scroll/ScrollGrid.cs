using UnityEngine;

public class ScrollGrid : IScrollLayout
{
	private int _column;
	private float _itemWidth;
	private float _itemHeight;

	public ScrollGrid(float w, float h, float viewWidth, int c = 0)
	{
		this._itemWidth = w;
		this._itemHeight = h;
		Debug.Log($"[ScrollGrid] viewWidth: {viewWidth}, itemWidth: {_itemWidth}, column: {_column}");

		if (c == 0) {
			this._column = Mathf.FloorToInt(viewWidth / w);
			this._column = Mathf.Max(this._column, 1);
		} else
			this._column = c;
	}

	public Vector2 GetItemPosition(int index)
	{
		int row = index / _column;
		int col = index % _column;
		return new Vector2(col * _itemWidth, -row * _itemHeight);
	}

	public Vector2 GetContentSize(int itemCount)
	{
		int rows = Mathf.CeilToInt((float)itemCount / _column);
		return new Vector2(0, rows * _itemHeight);
	}


	public int GetFirstVisibleIndex(float scrollY)
	{
		return Mathf.FloorToInt(scrollY / _itemHeight) * _column;
	}

	public int GetVisibleCount(float viewHeight, int buffCount)
	{
		return Mathf.CeilToInt(viewHeight / _itemHeight) * _column + buffCount;
	}
}
