using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookie : MonoBehaviour, IBattleUnit
{
    [SerializeField] private CookieData _cookieData;
    public CookieData CookieData => _cookieData;

    private bool isPerformingAction = false;

    public string UnitID => _cookieData.cookieID;

    public string DisplayName => _cookieData.displayName;

    public Vector3 Position { get => transform.position; set => transform.position = value; }
    public int CurrentHP { get; set; }

    public int MaxHP => _cookieData.baseHP;

    public int AttackPower => _cookieData.baseAttackPower;

    public int DefensePower => _cookieData.baseDefensePower;

    public float CriticalRate => _cookieData.baseCriticalRate;

    public float CriticalDamage => _cookieData.baseCriticalDamage;

    public float AttackCurCooltime { get; set; }

    public float AttackCooltime => _cookieData.attackCooltime;

    public Define.AttackType AttackType => _cookieData.attackType;

    public float AttackRange => _cookieData.attackRange;

    public Define.TargetPriority AttackTargetPriority => _cookieData.targetPriority;

    public float MoveSpeed => _cookieData.moveSpeed;

    public bool IsMoving { get; set; }

    public bool IsAlive => CurrentHP > 0;

    public Action<IBattleUnit> OnDead { get; set; }
    public Action<IBattleUnit, int, bool> OnDamaged { get; set; }
    public Action<IBattleUnit, IBattleUnit, int, bool> OnAttack { get; set; }

    public void Init()
    {
        CurrentHP = MaxHP;
        AttackCurCooltime = 0f;
        IsMoving = false;
        Position = transform.position;
    }

    public void SetCookieData(CookieData data)
    {
        _cookieData = data;
        Init();
    }

    public bool IsInAttackRange(IBattleUnit target)
    {
        float distance = Vector3.Distance(Position, target.Position);
        return distance <= AttackRange;
    }

    public bool IsInSkillRange(IBattleUnit target)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int damage, bool isCritical = false)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        OnDamaged?.Invoke(this, damage, isCritical);

        if (!IsAlive)
        {
            Debug.Log($"{DisplayName}이(가) 쓰러졌습니다!");
            OnDead?.Invoke(this);
        }
        else
        {
            // 피격 애니메이션 재생?
        }
    }

    public void TakeHeal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        Debug.Log($"{DisplayName}이(가) {amount}만큼 회복했습니다. 현재 HP: {CurrentHP}/{MaxHP}");
    }

    public bool TryNormalAttack(IBattleUnit target, System.Random battleRandom)
    {
        if (!IsAlive || target == null || !target.IsAlive)
            return false;
        
        if (AttackCooltime > 0f || isPerformingAction)
            return false; // 쿨타임이 남아있으면 공격 불가

        // 공격이 가능한 상태라면
        AttackCurCooltime = AttackCooltime; // 쿨타임 초기화
        isPerformingAction = true;

        // Heal 타입은 평타가 아군 힐
        if (AttackType == Define.AttackType.Heal)
        {
            // TODO: 공격 애니메이션 재생
            // animController.PlayAttackAnimation();
            PerformAttackHeal(target, battleRandom);
        }
        else
        {
            // TODO: 공격 애니메이션 재생
            // animController.PlayAttackAnimation();
            PerformAttackDamage(target, battleRandom);
        }
        isPerformingAction = false;

        return true;
    }

    private void PerformAttackHeal(IBattleUnit target, System.Random battleRandom)
    {
        // 크리티컬 판정
        bool isCritical = battleRandom.NextDouble() < CriticalRate; // 0~1 사이의 랜덤 값 생성

        // 힐량 계산
        float heal = AttackPower;
        if (isCritical) heal *= CriticalDamage;

        string criText = isCritical ? " [크리티컬!]" : "";
        Debug.Log($"{DisplayName}이(가) {target.DisplayName} 회복~ 회복량: {heal}{criText} (대상 HP: {target.CurrentHP}/{target.MaxHP})");


        target.TakeHeal(Mathf.RoundToInt(heal)); // 대상에게 힐 적용
    }

    private void PerformAttackDamage(IBattleUnit target, System.Random battleRandom)
    {
        // 크리티컬 판정
        bool isCritical = battleRandom.NextDouble() < CriticalRate; // 0~1 사이의 랜덤 값 생성

        // 데미지 계산
        float damage = AttackPower;
        if (isCritical) damage *= CriticalDamage;

        // 방어력 적용 (데미지 - 방어력), 최소뎀 1
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage - target.DefensePower));

        string criText = isCritical ? " [크리티컬!]" : "";
        Debug.Log($"{DisplayName}이(가) {target.DisplayName} 공격! 데미지: {finalDamage}{criText} (적 HP: {target.CurrentHP}/{target.MaxHP})");

        target.TakeDamage(finalDamage, isCritical); // 대상에게 데미지 적용
        OnAttack?.Invoke(this, target, finalDamage, isCritical);
    }

    public bool TrySkillAttack(List<IBattleUnit> targets, System.Random battleRandom)
    {
        throw new NotImplementedException();
    }

    public void UpdateCooltime(float deltaTime)
    {
        if (AttackCurCooltime > 0) AttackCurCooltime -= deltaTime;
    }
}
