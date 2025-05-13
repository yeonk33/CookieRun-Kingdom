using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
	public string buildingId;

	private void OnMouseDown() // Collider 필요
	{
		BuildingManager.Instance.OpenPanel();
	}
}
