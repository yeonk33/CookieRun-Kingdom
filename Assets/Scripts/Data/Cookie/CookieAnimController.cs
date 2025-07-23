using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class CookieAnimController : MonoBehaviour
{
    public SkeletonAnimation _skeletonAnim;

    [Header("현재 상태")]
    public AnimationState _currentState = AnimationState.Idle;
    public bool _isPlayingPLE = false;      // PLE 애니메이션 중인지

    private Cookie _cookie;
    private Coroutine _currentPLECoroutine = null;
    private bool _isFacingBack = false;

    private void Awake()
    {
        _cookie = GetComponent<Cookie>();

        if (_skeletonAnim == null)
        {
            _skeletonAnim = GetComponentInChildren<SkeletonAnimation>();
        }
    }

    public void SetFacingBack(bool isFacingBack)
    {
        _isFacingBack = isFacingBack;
    }

    private string GetDirectionAnimName(string baseName)
    {
        if (string.IsNullOrEmpty(baseName))
        {
            Debug.LogError("기본 애니메이션 이름이 비어 있습니다!");
            return baseName;
        }

        return _isFacingBack ? $"{baseName}_back" : baseName;
    }

    private void ApplyFlip()
    {
        if (_skeletonAnim && _skeletonAnim.skeleton != null)
        {
            _skeletonAnim.skeleton.ScaleX = _isFacingBack ? 1f : -1f;
        }
    }

    public void PlayAnimation(AnimationInfo animInfo, System.Action onComplete = null)
    {
        if (animInfo.IsPreLoopEnd)
        {
            PlayPreLoopEndAnimation(animInfo, onComplete);
        }
        else
        {
            PlayOneTimeAnimation(animInfo, onComplete);
        }
    }

    public void PlayPreLoopEndAnimation(AnimationInfo animInfo, System.Action onComplete = null)
    {
        StopCurrentPLE();

        _currentPLECoroutine = StartCoroutine(PlayPreLoopEndCoroutine(animInfo, onComplete));
    }

    public void PlayOneTimeAnimation(AnimationInfo animInfo, System.Action onComplete = null)
    {
        if (_skeletonAnim == null)
        {
            Debug.LogError("SkeletonAnimation 컴포넌트가 없습니다!");
            return;
        }

        if (string.IsNullOrEmpty(animInfo.animName))
        {
            Debug.LogWarning("애니메이션 이름이 비어 있습니다!");
            return;
        }

        // 진행 중인 PLE가 있다면 중단
        StopCurrentPLE();

        ApplyFlip();

        string animName = GetDirectionAnimName(animInfo.animName);

        var entry = _skeletonAnim.AnimationState.SetAnimation(0, animName, animInfo.isLoop);

        if (onComplete != null)
        {
            entry.Complete += (entry) => onComplete?.Invoke();
        }
    }

    private void StopCurrentPLE()
    {
        if (_currentPLECoroutine != null)
        {
            StopCoroutine(_currentPLECoroutine);
            _currentPLECoroutine = null;
            _isPlayingPLE = false;
        }
    }

    private IEnumerator PlayPreLoopEndCoroutine(AnimationInfo animInfo, System.Action onComplete)
    {
        _isPlayingPLE = true;

        ApplyFlip();

        // Pre
        if (!string.IsNullOrEmpty(animInfo.preAnim))
        {
            string preAnimName = GetDirectionAnimName(animInfo.preAnim);
            Debug.Log($"Pre 애니메이션 재생: {preAnimName}");
            yield return PlayAnimationAndWait(preAnimName, false);
        }

        // Loop
        if (!string.IsNullOrEmpty(animInfo.loopAnim))
        {
            string loopAnimName = GetDirectionAnimName(animInfo.loopAnim);
            Debug.Log($"Loop 애니메이션 재생: {loopAnimName} {animInfo.loopDuration}");
            _skeletonAnim.AnimationState.SetAnimation(0, loopAnimName, true);
            yield return new WaitForSeconds(animInfo.loopDuration);
        }

        // End
        if (!string.IsNullOrEmpty(animInfo.endAnim))
        {
            string endAnimName = GetDirectionAnimName(animInfo.endAnim);
            Debug.Log($"End 애니메이션 재생: {endAnimName}");
            yield return PlayAnimationAndWait(endAnimName, false);
        }

        _isPlayingPLE = false;

        onComplete?.Invoke();
    }

    private IEnumerator PlayAnimationAndWait(string animName, bool isLoop)
    {
        var entry = _skeletonAnim.AnimationState.SetAnimation(0, animName, isLoop);

        if (!isLoop)
        {
            bool animCompleted = false;
            entry.Complete += (entry) => animCompleted = true;

            yield return new WaitUntil(() => animCompleted);
        }
    }

    public void PlayAttackAnimation(CookieData cookieData, System.Action onComplete = null)
    {
        _currentState = AnimationState.Attack;
        PlayAnimation(cookieData.attackAnimationInfo, () =>
        {
            _currentState = AnimationState.Idle;
            onComplete?.Invoke();
        });
    }

    public void PlaySkillAnimation(CookieData cookieData, System.Action onComplete = null)
    {
        _currentState = AnimationState.Skill;
        PlayAnimation(cookieData.attackAnimationInfo, () =>
        {
            _currentState = AnimationState.Idle;
            onComplete?.Invoke();
        });
    }

    public void PlayIdleAnimation(CookieData cookieData)
    {
        if (_isPlayingPLE)
            return;

        _currentState = AnimationState.Idle;
        var idleAnim = AnimationInfo.CreateOneTime("battle_idle", true);
        PlayAnimation(idleAnim);
    }

    public void PlayRunAnimation(CookieData cookieData)
    {
        if (_isPlayingPLE)
            return;

        _currentState = AnimationState.Run;
        var idleAnim = AnimationInfo.CreateOneTime("run", true);
        PlayAnimation(idleAnim);
    }
}
