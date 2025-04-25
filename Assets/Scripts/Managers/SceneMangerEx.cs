using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMangerEx
{
	public BaseScene CurrentScene // �ܺο��� ���� �� get ����
	{
		get	{ return GameObject.FindObjectOfType<BaseScene>(); }
	}

	private string GetSceneName(Define.Scene s) // scene�� enum -> string ��ȯ
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
