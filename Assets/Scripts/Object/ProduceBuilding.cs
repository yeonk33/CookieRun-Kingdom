using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceBuilding : MonoBehaviour
{
	private void OnMouseDown() // Collider �ʿ�
	{
		Debug.Log("building click");
		BuildingManager.Instance.OpenPanel();
	}
}
