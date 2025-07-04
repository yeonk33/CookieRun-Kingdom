using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    public Vector3 pivot;

    private void Awake()
    {
        pivot = transform.position - transform.Find("Pivot").position;
        Debug.Log(pivot);
    }
}
