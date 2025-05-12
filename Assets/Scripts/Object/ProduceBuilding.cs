using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
	private void OnMouseDown() // Collider ÇÊ¿ä
	{
		BuildingManager.Instance.OpenPanel();
	}
}
