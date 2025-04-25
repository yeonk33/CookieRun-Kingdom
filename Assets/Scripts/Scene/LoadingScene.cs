using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : BaseScene
{
	private Define.Scene nextScene;
	protected override void Init()
	{
		base.Init();
		Scene = Define.Scene.LoadingScene;
		Debug.Log($"{Scene}");

		nextScene = Define.Scene.KingdomScene;
		//Managers.Scene.LoadScene(nextScene);
	}

	public void StartButton()
	{
		Managers.Scene.LoadScene(nextScene);
	}
}
