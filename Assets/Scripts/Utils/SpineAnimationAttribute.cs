using UnityEngine;
using UnityEditor;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;

public class SpineAnimationAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(SpineAnimationAttribute))]
public class SpineAnimationPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 애니메이션 리스트 가져오기
        var animationNames = GetAnimationNames(property);

        if (animationNames.Length <= 1) // "[None]"만 있는 경우
        {
            // 애니메이션을 찾을 수 없으면 일반 텍스트 필드로 표시
            EditorGUI.PropertyField(position, property, label);
        }
        else
        {
            // 드롭다운으로 표시
            DrawAnimationDropdown(position, property, label, animationNames);
        }

        EditorGUI.EndProperty();
    }

    private void DrawAnimationDropdown(Rect position, SerializedProperty property, GUIContent label, string[] animationNames)
    {
        // 현재 선택된 애니메이션 찾기
        int currentIndex = 0; // 기본값은 "[None]"

        if (!string.IsNullOrEmpty(property.stringValue))
        {
            currentIndex = System.Array.IndexOf(animationNames, property.stringValue);
            if (currentIndex == -1) currentIndex = 0; // 찾을 수 없으면 "[None]" 선택
        }

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, animationNames);

        if (EditorGUI.EndChangeCheck())
        {
            // "[None]"이 선택되면 빈 문자열로 저장
            if (newIndex == 0)
            {
                property.stringValue = "";
            }
            else if (newIndex >= 0 && newIndex < animationNames.Length)
            {
                property.stringValue = animationNames[newIndex];
            }
        }
    }

    private string[] GetAnimationNames(SerializedProperty property)
    {
        SkeletonDataAsset skeletonDataAsset = null;

        // 1. 먼저 직접 skeletonDataAsset 필드 찾기 (직접 참조가 있는 경우)
        var skeletonDataProperty = property.serializedObject.FindProperty("skeletonDataAsset");
        if (skeletonDataProperty?.objectReferenceValue is SkeletonDataAsset asset)
        {
            skeletonDataAsset = asset;
        }
        // 2. prefab 필드에서 찾기 (CookieData인 경우)
        else if (TryGetFromPrefab(property, out skeletonDataAsset))
        {
            // prefab에서 찾았음
        }
        // 3. SkillData인 경우 - 이 SkillData를 사용하는 CookieData 찾기
        else if (property.serializedObject.targetObject is SkillData skillData)
        {
            skeletonDataAsset = FindSkeletonDataAssetFromSkillData(skillData);
        }

        if (skeletonDataAsset != null)
        {
            return GetAnimationNamesFromSkeletonData(skeletonDataAsset);
        }

        return new string[] { "[None]" };
    }

    private bool TryGetFromPrefab(SerializedProperty property, out SkeletonDataAsset skeletonDataAsset)
    {
        skeletonDataAsset = null;

        var prefabProperty = property.serializedObject.FindProperty("prefab");
        if (prefabProperty?.objectReferenceValue is GameObject prefab)
        {
            var skeletonAnim = GetSkeletonAnimationFromPrefab(prefab);
            skeletonDataAsset = skeletonAnim?.skeletonDataAsset;
            return skeletonDataAsset != null;
        }

        return false;
    }

    private SkeletonDataAsset FindSkeletonDataAssetFromSkillData(SkillData skillData)
    {
        // 프로젝트에서 이 SkillData를 참조하는 CookieData 찾기
        string[] cookieDataGuids = AssetDatabase.FindAssets("t:CookieData");

        foreach (string guid in cookieDataGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CookieData cookieData = AssetDatabase.LoadAssetAtPath<CookieData>(path);

            if (cookieData != null && cookieData.skillData == skillData)
            {
                // 이 CookieData가 해당 SkillData를 사용함!
                if (cookieData.prefab != null)
                {
                    var skeletonAnim = GetSkeletonAnimationFromPrefab(cookieData.prefab);
                    return skeletonAnim?.skeletonDataAsset;
                }
            }
        }

        return null;
    }

    private SkeletonAnimation GetSkeletonAnimationFromPrefab(GameObject prefab)
    {
        // 1. "Skeleton" 자식 오브젝트에서 찾기
        var skeletonChild = prefab.transform.Find("Skeleton");
        if (skeletonChild != null)
        {
            var skeletonAnim = skeletonChild.GetComponent<SkeletonAnimation>();
            if (skeletonAnim != null) return skeletonAnim;
        }

        // 2. 전체에서 찾기
        return prefab.GetComponentInChildren<SkeletonAnimation>();
    }

    private string[] GetAnimationNamesFromSkeletonData(SkeletonDataAsset skeletonDataAsset)
    {
        try
        {
            var skeletonData = skeletonDataAsset.GetSkeletonData(false);
            if (skeletonData?.Animations != null)
            {
                var names = new List<string> { "[None]" }; // [None] 옵션을 첫 번째에 추가
                names.AddRange(skeletonData.Animations.Select(anim => anim.Name));
                return names.ToArray();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to get animation names from SkeletonDataAsset: {e.Message}");
        }

        return new string[] { "[None]" };
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}