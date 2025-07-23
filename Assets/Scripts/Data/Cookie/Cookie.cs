using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Cookie : MonoBehaviour, IBattleUnit
{
    [SerializeField]
    private CookieData cookieData;
    public CookieAnimController animController;

    private bool isPerformingAction = false;

    public CookieData CData => cookieData;

    public string UnitID => cookieData.cookieID;
    public string DisplayName => cookieData.displayName;
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public int CurrentHP { get; set; }
    public int MaxHP => cookieData.baseHP;
    public int AttackPower => cookieData.baseAttackPower;
    public int DefensePower => cookieData.baseDefensePower;
    public float CriticalRate => cookieData.baseCriticalRate;
    public float CriticalDamage => cookieData.baseCriticalDamage;

    public float AttackCurCooltime { get; set; }
    public float AttackCooltime => cookieData.attackCooltime;
    public AttackType AttackType => cookieData.attackType;
    public float AttackRange => cookieData.attackRange;
    public TargetPriority AttackTargetPriority => cookieData.targetPriority;

    public SkillData SkillData => cookieData.skillData;
    public float SkillCurCooltime { get; set; }

    public float MoveSpeed => cookieData.moveSpeed;
    public bool IsMoving { get; set; }

    public bool IsAlive => CurrentHP > 0;

    // 디버프 정보 - 클래스
    //public DebuffInfo currentDebuff;
    // 내부에는 디버프 타입, 디버프 초, 타입별 디버프 효과(어떤 스탯 감소, 스턴,,,,,)

    public Action<IBattleUnit> OnDead { get; set; }
    public Action<IBattleUnit, int, bool> OnDamaged { get; set; }
    public Action<IBattleUnit, IBattleUnit, int, bool> OnAttack { get; set; }
    public Action<IBattleUnit, SkillData> OnSkillUsed { get; set; }

    private void Awake()
    {
        if (animController == null)
        {
            animController = gameObject.GetOrAddComponent<CookieAnimController>();
        }
    }

    public void Init()
    {
        CurrentHP = MaxHP;
        AttackCurCooltime = 0f;
        SkillCurCooltime = SkillData.cooltime;
        IsMoving = false;
        isPerformingAction = false;
        Position = transform.position;
        animController.PlayIdleAnimation(cookieData);
    }

    public void SetCookieData(CookieData data)
    {
        cookieData = data;
        Init();
    }

    public bool IsInAttackRange(IBattleUnit target)
    {
        float distance = Vector3.Distance(Position, target.Position);
        return distance <= AttackRange;
    }

    public bool IsInSkillRange(IBattleUnit target)
    {
        return IsInAttackRange(target);     // 쿠키는 기본적으로 평타와 동일한 범위로 설정
    }

    public void TakeDamage(int damage, bool isCritical = false)
    {
        int oldHP = CurrentHP;

        CurrentHP = Mathf.Max(0, CurrentHP - damage);

        OnDamaged?.Invoke(this, damage, isCritical);

        // 죽음 애니메이션
        if (IsAlive)
        {
            /*
            animController.PlayHitAnimation(cookieData, () =>
            {
                if (IsAlive)
                    animController.PlayIdleAnimation(cookieData);
                else
                    animController.PlayDeadAnimation(cookieData, () => { OnDead?.Invoke(this); });
            });
            */
        }
        else
        {
            Debug.Log($"{DisplayName}이(가) 쓰려졌습니다!");
            OnDead?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void TakeHeal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    public bool TryNormalAttack(IBattleUnit target, System.Random battleRandom)
    {
        if (!IsAlive || target == null || !target.IsAlive)
            return false;

        if (AttackCurCooltime > 0 || isPerformingAction)
            return false;

        AttackCurCooltime = AttackCooltime;
        isPerformingAction = true;

        // 지금은 Complete 타이밍에 무조건 데미지가 들어가는데,
        // 사양에 따라서 이런 구조 말고, N초 뒤에 코루틴으로 데미지를 처리한다 <- 이런 구조도 가능하다!
        animController.PlayAttackAnimation(cookieData, () =>
        {
            PerformAttackDamage(target, battleRandom);
            isPerformingAction = false;

            // 공격 후 Idle로 돌아가기
            if (IsAlive)
                animController.PlayIdleAnimation(cookieData);
        });

        return true;
    }

    public bool TrySkillAttack(List<IBattleUnit> targets, System.Random battleRandom)
    {
        if (!IsAlive || targets == null || targets.Count == 0)
            return false;

        if (SkillCurCooltime > 0 || isPerformingAction)     // 만약 Attack 애니메이션을 캔슬할 수 있다면 조건이 달라져야 한다!
            return false;

        SkillCurCooltime = SkillData.cooltime;
        isPerformingAction = true;

        animController.PlaySkillAnimation(cookieData, () =>
        {
            PerformSkillDamage(targets, battleRandom);
            isPerformingAction = false;

            if (IsAlive)
                animController.PlayIdleAnimation(cookieData);
        });

        return true;
    }

    private void PerformAttackDamage(IBattleUnit target, System.Random battleRandom)
    {
        // 크리티컬 판정
        bool isCritical = battleRandom.NextDouble() < CriticalRate;

        // 데미지 계산
        float damage = AttackPower;
        if (isCritical)
        {
            damage *= CriticalDamage;
        }

        int finalDamage = Mathf.Max(0, (int)damage - target.DefensePower);  // 방어력 반영, 구하는 공식 바꿔도 됨

        string criText = isCritical ? " [크리티컬!]" : "";
        Debug.Log($"{DisplayName}이(가) {target.DisplayName} 공격! 데미지: {finalDamage}{criText} (적 HP: {target.CurrentHP}/{target.MaxHP})");

        target.TakeDamage(finalDamage, isCritical);

        OnAttack?.Invoke(this, target, finalDamage, isCritical);
    }

    private void PerformSkillDamage(List<IBattleUnit> targets, System.Random battleRandom)
    {
        foreach (var target in targets.Where(t => t.IsAlive))
        {
            if (SkillData.skillType == SkillType.Damage)
            {
                // 크리티컬 판정
                bool isCritical = battleRandom.NextDouble() < CriticalRate;

                // 데미지 계산
                float damage = AttackPower * SkillData.damageMultiplier;
                if (isCritical)
                {
                    damage *= CriticalDamage;
                }

                int finalDamage = Mathf.Max(0, (int)damage - target.DefensePower);  // 방어력 반영, 구하는 공식 바꿔도 됨

                int currentHP = Mathf.Max(0, target.CurrentHP - finalDamage);
                string criText = isCritical ? " [크리티컬!]" : "";

                Debug.Log($"{DisplayName}이(가) {target.DisplayName} 스킬 공격! 데미지: {finalDamage}{criText} (적 HP: {currentHP}/{target.MaxHP})");

                target.TakeDamage(finalDamage, isCritical);
            }
            else if (SkillData.skillType == SkillType.Heal)
            {
                int healAmount = Mathf.Max(0, (int)(AttackPower * SkillData.damageMultiplier));
                target.TakeHeal(healAmount);
                Debug.Log($"{DisplayName}이(가) {target.DisplayName}을(를) 치유! 치유량: {healAmount} (아군 HP: {target.CurrentHP}/{target.MaxHP})");
            }
            // 만약에 이 스킬이 디버프를 가지고 있음
            // 타겟에다가 디버프 세팅
            // target.SetDebuff(debuffInfo);
        }

        OnSkillUsed?.Invoke(this, SkillData);
    }

    public void UpdateCooltime(float deltaTime)
    {
        if (AttackCurCooltime > 0)
            AttackCurCooltime -= deltaTime;

        if (SkillCurCooltime > 0)
            SkillCurCooltime -= deltaTime;
    }
}
