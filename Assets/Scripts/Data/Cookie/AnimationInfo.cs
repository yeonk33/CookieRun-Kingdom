using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationInfo
{
    [Header("OneTime 애니메이션")]
    [SpineAnimationAttribute]
    public string animName;     // 애니메이션 이름 (Spine 애니메이션 이름과 동일)
    public bool isLoop;         // 오버라이드용

    [Header("PreLoopEnd 애니메이션")]
    [SpineAnimationAttribute]
    public string preAnim;      // skill1
    [SpineAnimationAttribute]
    public string loopAnim;     // skill1_loop
    [SpineAnimationAttribute]
    public string endAnim;      // skill1_end
    public float loopDuration;

    public bool IsPreLoopEnd => !string.IsNullOrEmpty(loopAnim);

    public static AnimationInfo CreateOneTime(string animName, bool isLoop = false)
    {
        return new AnimationInfo
        {
            animName = animName,
            isLoop = isLoop,
            preAnim = "",
            loopAnim = "",
            endAnim = "",
            loopDuration = 0f
        };
    }

    public static AnimationInfo CreatePreLoopEnd(string preAnim, string loopAnim, string endAnim, float loopTime)
    {
        return new AnimationInfo
        {
            animName = "",
            isLoop = false,
            preAnim = preAnim,
            loopAnim = loopAnim,
            endAnim = endAnim,
            loopDuration = loopTime
        };
    }
}
