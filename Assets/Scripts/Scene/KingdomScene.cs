using UnityEngine;

public class KingdomScene : BaseScene
{
	protected override void Init()
	{
		base.Init();

		// KingdomScene에서 처리해야할 Init 작업들
		Scene = Define.Scene.KingdomScene;
		Debug.Log($"current scene {Scene}");
	}
}
