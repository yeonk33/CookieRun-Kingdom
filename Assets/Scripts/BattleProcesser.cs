using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class BattleProcesser : MonoBehaviour
{
    [Header("전투 유닛 설정")]
    private List<IBattleUnit> _playerTeam;
    private List<IBattleUnit> _enemyTeam;

    [Header("시간 관련")]
    private float _battleTime = 0f;
    private const float TIMESTAMP = 0.02f; // 50 FPS
    private bool _isBattleActive = false;

    [Header("결정론적 시스템")]
    private System.Random _randomGenerator;
    private int _seed;

    private void InitBattle(int seed)
    {
        _seed = seed == -1 ? UnityEngine.Random.Range(0, int.MaxValue) : seed;
        _randomGenerator = new System.Random(_seed);
        _battleTime = Time.fixedDeltaTime;         // 0.02f (50 fps)

        Debug.Log($"시드값 : {_seed}");
    }

    public void StartPVPBattle(List<IBattleUnit> playerTeam, List<IBattleUnit> enemyTeam, int seed = -1)
    {
        InitBattle(seed);

        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;

        InitTeamPosition(_playerTeam, true);
        InitTeamPosition(_enemyTeam, false);

        foreach (var unit in _playerTeam)
        {
            unit.Init();
        }

        foreach (var unit in _enemyTeam)
        {
            unit.Init();
        }

        _isBattleActive = true;
    }

    private void InitTeamPosition(List<IBattleUnit> team, bool isPlayerTeam)
    {
        float offset = isPlayerTeam ? -10.0f : 10.0f;
        float spacing = 2.0f;

        for (int i = 0; i < team.Count; ++i)
        {
            Vector3 position = new Vector3(offset, (i - 2) * spacing, 0);
            team[i].Position = position;

            if (team[i] is Cookie cookie)
            {
                cookie.animController.SetFacingBack(isPlayerTeam);
            }
        }
    }

    void FixedUpdate()
    {
        if (!_isBattleActive || IsBattleOver())
            return;

        ProcessTeamActions(_playerTeam, _enemyTeam);
        ProcessTeamActions(_enemyTeam, _playerTeam);
    }

    private void ProcessTeamActions(List<IBattleUnit> team, List<IBattleUnit> enemyTeam)
    {
        foreach (var unit in team.Where(u => u.IsAlive))
        {
            // 쿨타임을 먼저 줄여 준다
            unit.UpdateCooltime(_battleTime);

            // 타겟팅 한다
            var target = FindAttackTarget(unit, enemyTeam);
            if (target == null)
                continue;

            // 만약 내가 스턴이라면 continue

            // 만약 내가 지금 도트힐, 도트딜 중이라면 => 함수 호출

            // 사거리 체크, 만약 사거리가 안 됐다면 이동
            if (!unit.IsInAttackRange(target))
            {
                MoveToTarget(unit, target);
                continue;
            }

            // 사거리 안쪽으로 이동헀다면, 애니메이션 멈춤
            if (unit.IsMoving)
            {
                StopMoving(unit);
            }

            // 스킬을 먼저 사용
            if (unit.SkillCurCooltime <= 0f && unit.SkillData != null)
            {
                var skillTargets = FindSkillTargets(unit, enemyTeam);
                if (skillTargets.Count > 0)
                {
                    unit.TrySkillAttack(skillTargets, _randomGenerator);
                }
                continue;
            }

            // 평타를 사용
            if (unit.AttackCurCooltime <= 0f)
            {
                unit.TryNormalAttack(target, _randomGenerator);
            }
        }
    }

    private void MoveToTarget(IBattleUnit unit, IBattleUnit target)
    {
        Vector3 direction = (target.Position - unit.Position).normalized;
        float moveDistance = unit.MoveSpeed * TIMESTAMP;

        unit.Position += direction * moveDistance;

        if (!unit.IsMoving)
        {
            unit.IsMoving = true;
            if (unit is Cookie cookie)
            {
                cookie.animController.PlayRunAnimation(cookie.CData);
            }
        }
    }

    private void StopMoving(IBattleUnit unit)
    {
        unit.IsMoving = false;
        if (unit is Cookie cookie)
        {
            cookie.animController.PlayIdleAnimation(cookie.CData);
        }
    }

    private IBattleUnit FindAttackTarget(IBattleUnit attacker, List<IBattleUnit> enemyTeam, TargetPriority targetPriority = TargetPriority.None)
    {
        var aliveEnemies = enemyTeam.Where(unit => unit.IsAlive).ToList();
        if (aliveEnemies.Count == 0)        // == if (aliveEnemies.Any())
            return null;

        TargetPriority priority = attacker.AttackTargetPriority;
        if (targetPriority != TargetPriority.None)
            priority = targetPriority;

        switch (priority)
        {
            case TargetPriority.Nearest:
                return aliveEnemies.OrderBy(e => Vector3.Distance(attacker.Position, e.Position)).First();
            case TargetPriority.Farthest:
                return aliveEnemies.OrderByDescending(e => Vector3.Distance(attacker.Position, e.Position)).First();
            case TargetPriority.LowestHP:
                return aliveEnemies.OrderBy(e => e.CurrentHP).First();
            default:
                return aliveEnemies.First();
        }
    }

    private List<IBattleUnit> FindSkillTargets(IBattleUnit caster, List<IBattleUnit> enemyTeam)
    {
        var targets = new List<IBattleUnit>();
        var aliveEnemies = enemyTeam.Where(unit => unit.IsAlive).ToList();

        if (caster.SkillData == null)
            return targets;

        switch (caster.SkillData.skillTargetType)
        {
            case SkillTargetType.Single:
                var singleTarget = FindAttackTarget(caster, enemyTeam, caster.SkillData.targetPriority);
                if (singleTarget != null)
                    targets.Add(singleTarget);
                break;
            case SkillTargetType.MultipleEnemies:
                targets = aliveEnemies.OrderBy(e => Vector3.Distance(caster.Position, e.Position)).Take(3).ToList();
                break;
            case SkillTargetType.AllEnemies:
                targets = aliveEnemies;
                break;
        }

        return targets;
    }

    public bool IsBattleOver()
    {
        return _playerTeam.All(unit => !unit.IsAlive) || _enemyTeam.All(unit => !unit.IsAlive);
    }
}