using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
	[SerializeField] private string _buildingId;
	[SerializeField] private int _level;
	private BuildingData _data;


	public void SetBuildingData(string id, int lv)
	{
		_buildingId = id;
		_level = lv;
		_data = BuildingDatabase.Get(id);
		if (_data == null) Debug.LogError($"{id} 생산 건물 데이터 찾을 수 없음");
		this.GetOrAddComponent<SpriteRenderer>().sprite = _data.icon;
	}

	private void OnMouseDown() // Collider 필요
	{
		//BuildingManager.Instance.OpenPanel(_data, _level);
		ProductionPanel.Instance.OpenPanel(_data, _level);
	}
}
