using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static event Action<BuildingData> OnBuildingPurchased;

    public static void TriggerBuildingPurchased(BuildingData building)
    {
        OnBuildingPurchased?.Invoke(building);
    }
}
