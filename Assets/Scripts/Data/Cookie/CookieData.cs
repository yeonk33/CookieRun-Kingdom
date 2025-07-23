using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "NewCookie", menuName = "Game/CookieData")]
public class CookieData : ScriptableObject
{
    [Header("기본 정보")]
    public string cookieID;
    public string displayName;

    [Header("기본 스탯")]
    public int baseHP;
    public int baseAttackPower;
    public int baseDefensePower;

    public float attackCooltime = 2.0f; // 기본 평타 쿨타임 (초 단위)
    public float attackRange = 1.0f;    // 평타 공격 범위 (유닛 간 거리)
    public AttackType attackType = AttackType.Melee; // 평타 공격 타입 (근거리, 원거리 등)
    public TargetPriority targetPriority = TargetPriority.Nearest; // 평타 공격 대상 우선순위

    public float moveSpeed = 5.0f;          // 이동 속도 (초당 거리)

    public float baseCriticalRate = 0.1f;   // 기본 크리티컬 확률 (10%)
    public float baseCriticalDamage = 1.5f; // 기본 크리티컬 데미지 배율 (150%)

    [Header("스킬 데이터")]
    public SkillData skillData;

    [Header("평타 애니메이션")]
    public AnimationInfo attackAnimationInfo;    // 평타 애니메이션 정보

    public GameObject prefab;
    public Sprite icon;
}