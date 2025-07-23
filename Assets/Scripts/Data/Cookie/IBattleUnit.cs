using System.Collections.Generic;
using UnityEngine;

public interface IBattleUnit
{
    // 기본 정보
    string UnitID { get; }
    string DisplayName { get; }
    Vector3 Position { get; set; }

    // 전투 스탯
    int CurrentHP { get; set; }             // 전투 중 현재 HP
    int MaxHP { get; }
    int AttackPower { get; }
    int DefensePower { get; }
    float CriticalRate { get; }
    float CriticalDamage { get; }

    // 평타 관련
    float AttackCurCooltime { get; set; }   // 현재 평타 쿨타임
    float AttackCooltime { get; }           // 데이터상 평타 쿨타임
    Define.AttackType AttackType { get; }
    float AttackRange { get; }
    Define.TargetPriority AttackTargetPriority { get; }    // 평타 공격 대상 우선순위

    // 스킬 관련
    SkillData SkillData { get; }
    float SkillCurCooltime { get; set; }    // 현재 스킬 쿨타임

    // 이동 관련
    float MoveSpeed { get; }
    bool IsMoving { get; set; }

    // 상태
    bool IsAlive { get; }

    // 초기화
    void Init();

    // 전투 행동
    bool TryNormalAttack(IBattleUnit target, System.Random battleRandom);
    bool TrySkillAttack(List<IBattleUnit> targets, System.Random battleRandom);
    void TakeDamage(int damage, bool isCritical = false);
    void TakeHeal(int amount);

    // 타겟팅
    bool IsInAttackRange(IBattleUnit target);
    bool IsInSkillRange(IBattleUnit target);

    // 업데이트
    void UpdateCooltime(float deltaTime);

    // 이벤트
    System.Action<IBattleUnit> OnDead { get; set; }
    System.Action<IBattleUnit, int, bool> OnDamaged { get; set; } // unit, damage, isCritical
    System.Action<IBattleUnit, IBattleUnit, int, bool> OnAttack { get; set; } // attacker, target, damage, isCritical
    //System.Action<IBattleUnit, SkillData> OnSkillUsed { get; set; }
}
