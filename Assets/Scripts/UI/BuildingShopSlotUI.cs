using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingShopSlotUI : MonoBehaviour, IScrollSlot
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _cost;

    private BuildingData _building;

    public void SetUI(string id)
    {
        _building = BuildingDatabase.Get(id);
        if (_building == null)
        {
            Debug.LogError($"Building with ID {id} not found in the database.");
            return;
        }
        _image.sprite = _building.icon;
        _name.text = _building.displayName;
        _cost.text = _building.buildingLevels[0].coinCost.ToString("N0");
    }

    public void PurchaseClick()
    {
        EventManager.TriggerBuildingPurchased(_building);
    }

    #region IScrollSlot Implementation
    public void SetUIWithData(string id, object data)
    {
    }
    #endregion
}
