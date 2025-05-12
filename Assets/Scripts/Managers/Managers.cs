using UnityEngine;

public class Managers : MonoBehaviour
{
	private static Managers s_instance;
	public static Managers Instance
	{
		get
		{
			if (s_instance == null) {
				Debug.Log("find Managers");
				s_instance = FindFirstObjectByType<Managers>(); // Managers 없으면 찾기
				if (s_instance == null) {
					Debug.Log("Managers 없음 새로 생성");
					GameObject go = new GameObject("Managers");
					s_instance = go.AddComponent<Managers>(); // 그래도 없으면 만들기
					DontDestroyOnLoad(go);
				}
			}
			return s_instance;
		}
	}

	private GameManager _game;
	private SceneMangerEx _scene;
	public static GameManager Game => Instance._game ??= new GameManager(); // 사용될 때 생성
	public static SceneMangerEx Scene => Instance._scene ??= new SceneMangerEx();
}
