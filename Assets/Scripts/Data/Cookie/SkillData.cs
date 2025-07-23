using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "SkillData", menuName = "Game/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("스킬 정보")]
    public string skillID;
    public string skillName;
    public string skillDescription;

    [Header("스킬 쿨타임")]
    public float cooltime;                     // 기본 쿨타임 (초 단위)

    [Header("스킬 타겟팅")]
    public SkillType skillType;                // 스킬의 속성 (피해, 치유, 버프 등)
    public SkillTargetType skillTargetType;    // 스킬 타겟 타입 (단일, 다수 적, 모든 적, 단일 아군 등)
    public TargetPriority targetPriority;      // 타겟 우선순위 (가장 가까운 적, 가장 먼 적, HP가 가장 낮은 적 등)
    public float skillRange = 10.0f;          // 스킬 타겟팅 범위 (초 단위, 기본값: 10.0)

    [Header("스킬 데미지")]
    public float damageMultiplier = 1.0f;      // 데미지 배율 (기본값: 1.0), 쿠키 공격력 기반으로 계산됨

    [Header("애니메이션")]
    public AnimationInfo animationInfo;        // 스킬 애니메이션 정보
}