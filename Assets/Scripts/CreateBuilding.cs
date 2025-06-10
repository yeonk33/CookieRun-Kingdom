using Unity.VisualScripting;
using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
	[BuildingIdAttribute]
	[SerializeField] private string id;
	private void Start()
	{
		Create(id, 1);
	}
	
	public void Create(string id, int level)
	{
		var prefab = Resources.Load<GameObject>("Prefabs/ProductionBuilding");
		if (prefab == null) {
			Debug.LogError($"{id} 프리팹 로드 실패");
			return;
		}

		var data = BuildingDatabase.Get(id);
		GameObject go = GameObject.Instantiate(prefab); // prefab을 복사 생성한 후 정보 수정
		var pb = go.GetOrAddComponent<ProduceBuilding>();
		pb.SetBuildingData(data, level);
		//GameObject.Instantiate(go);
	}
}
