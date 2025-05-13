using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemIdAttribute))]
public class ItemIdDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//base.OnGUI(position, property, label);

		string[] allIds = ItemId.All;
		int currentIndex = Array.IndexOf(allIds, property.stringValue);
		if (currentIndex < 0) {
			currentIndex = 0; // default값 0으로 설정

		}

		int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, allIds);
		property.stringValue = allIds[selectedIndex];
	}
}
