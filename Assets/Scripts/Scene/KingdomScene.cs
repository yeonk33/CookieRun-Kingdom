using UnityEngine;

public class KingdomScene : BaseScene
{
	protected override void Init()
	{
		base.Init();

		// KingdomScene���� ó���ؾ��� Init �۾���
		Scene = Define.Scene.KingdomScene;
		Debug.Log($"current scene {Scene}");
	}
}
