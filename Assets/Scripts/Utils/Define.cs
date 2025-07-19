
public class Define
{
	public enum Scene
	{
		None,
		LoadingScene,
		KingdomScene,
	}

	public enum UIType
	{
		None,
		LoadingUI,
		Town,
		Produce,
		Inventory,
		BuildingShop,
		EditMode,
		EditPopup,
    }

    public enum AttackType
    {
        Melee,  // 근거리
        Ranged, // 원거리
        Heal,   // 회복
    }

    public enum TargetPriority
    {
        None,
        Nearest,    // 가장 가까운 적 (기본)
        Farthest,   // 가장 먼 적
        LowestHP,   // HP가 가장 낮은 적
    }
}
