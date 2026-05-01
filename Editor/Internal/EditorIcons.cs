// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class EditorIcons
	{
		public static readonly Texture2D NULL_ICON = (Texture2D)EditorGUIUtility.Load("Warning@2x");
		public static readonly Texture2D SCRIPT_ICON = (Texture2D)EditorGUIUtility.Load("cs Script Icon");

		public static Texture2D GetIcon(string name) => (Texture2D)EditorGUIUtility.Load(name);
	}
}