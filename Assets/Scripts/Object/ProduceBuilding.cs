using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
	private void OnMouseDown() // Collider �ʿ�
	{
		BuildingManager.Instance.OpenPanel();
	}
}
