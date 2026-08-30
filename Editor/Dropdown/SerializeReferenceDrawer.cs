// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	[CustomPropertyDrawer(typeof(SerializeReferenceAttribute))]
	public sealed class SerializeReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();

			if (SerializeReferenceUtility.DrawTypeDropdown(position, property, (SerializeReferenceAttribute)attribute))
			{
				EditorGUI.PropertyField(position, property, label, true);
			}

			EditorGUI.EndProperty();
			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);
	}
}