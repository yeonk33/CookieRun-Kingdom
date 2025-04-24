using UnityEngine;

public class BaseScene : MonoBehaviour
{
	public Define.Scene Scene { get; protected set; } = Define.Scene.None;

	private void Awake()
	{
		Init();
	}

	protected virtual void Init()
	{
		Debug.Log("BaseScene Init");
	}

	public virtual void Clear()
	{
		Debug.Log("BaseScene Clear");
	}
}
