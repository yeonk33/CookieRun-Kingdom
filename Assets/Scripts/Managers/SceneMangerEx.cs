using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMangerEx
{
	public BaseScene CurrentScene // 외부에서 현재 씬 get 가능
	{
		get	{ return GameObject.FindObjectOfType<BaseScene>(); }
	}

	private string GetSceneName(Define.Scene s) // scene을 enum -> string 변환
	{
		return System.Enum.GetName(typeof(Define.Scene), s);
	}

	public void LoadScene(Define.Scene scene)
	{
		string sceneName = GetSceneName(scene);

		if (SceneManager.GetActiveScene().name == sceneName)
			return;

		SceneManager.LoadScene(sceneName);
	}
}
