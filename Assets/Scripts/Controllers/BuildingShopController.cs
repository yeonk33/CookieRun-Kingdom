using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingShopController : MonoBehaviour, IPanelUI
{
    private List<BuildingShopSlotUI> slots;
    [SerializeField] private RectTransform _slotPrefab;
    [SerializeField] private RectTransform _contentPanel;

    public Define.UIType Type => Define.UIType.BuildingShop;

    void Start()
    {
        Init();
    }

    private void OnDestroy()
    {

    }

    public void Init()
    {
        slots = new List<BuildingShopSlotUI>();
        slots = GetComponentsInChildren<BuildingShopSlotUI>().ToList();
        if (slots.Count == 0)
        {
            Debug.LogError("No BuildingShopSlotUI components found in children.");
            return;
        }
        slots[0].SetUI(BuildingId.LumberjacksLodge);

        EventManager.OnBuildingPurchased += HandleOnPurchase;

        SetContentSize();
    }

    private void SetContentSize()
    {
        float sp = _contentPanel.gameObject.GetComponent<HorizontalLayoutGroup>().spacing;
        float w = (_slotPrefab.rect.width + sp) * slots.Count;
        _contentPanel.offsetMin = new Vector2(0, 0);
        _contentPanel.offsetMax = new Vector2(w - Screen.width, 0);
    }
    
    public void ClosePanel()
    {
        //gameObject.SetActive(false);
        UIManager.Instance.HideUI(Type);
    }

    private void HandleOnPurchase(BuildingData building)
    {
        ClosePanel();
    }

    public void ShowPanel()
    {
        UIManager.Instance.ShowUI(Type);
    }
}
