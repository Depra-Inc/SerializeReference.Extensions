// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class EditorIcons
	{
		public static readonly GUIContent NULL_ICON = EditorGUIUtility.IconContent("Warning@2x");
		public static readonly GUIContent SCRIPT_ICON = EditorGUIUtility.IconContent("cs Script Icon");
		public static readonly GUIContent MISSING_ICON = EditorGUIUtility.IconContent("Error@2x");
		public static readonly GUIContent DROPDOWN_ICON = EditorGUIUtility.IconContent("d_Linked");
	}
}