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
				s_instance = FindFirstObjectByType<Managers>(); // Managers ������ ã��
				if (s_instance == null) {
					Debug.Log("Managers ���� ���� ����");
					GameObject go = new GameObject("Managers");
					s_instance = go.AddComponent<Managers>(); // �׷��� ������ �����
					DontDestroyOnLoad(go);
				}
			}
			return s_instance;
		}
	}

	private GameManager _game;
	private SceneMangerEx _scene;
	public static GameManager Game => Instance._game ??= new GameManager(); // ���� �� ����
	public static SceneMangerEx Scene => Instance._scene ??= new SceneMangerEx();
}
