// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class EditorIcons
	{
		public static readonly Texture2D NULL_ICON = (Texture2D)EditorGUIUtility.Load("Warning@2x");
		private static readonly Texture2D SCRIPT_ICON = (Texture2D)EditorGUIUtility.Load("cs Script Icon");

		public static Texture2D GetIcon(string name) => (Texture2D)EditorGUIUtility.Load(name);

		public static Texture2D GetIcon(Type type) => GetIcon(type, SCRIPT_ICON);

		private static Texture2D GetIcon(Type type, Texture2D @default)
		{
			var guids = AssetDatabase.FindAssets($"{type.Name} t:script");
			if (guids.Length == 0)
			{
				return @default;
			}

			var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
			var importer = AssetImporter.GetAtPath(assetPath);
			var icon = importer is MonoImporter monoImporter ? monoImporter.GetIcon() ?? @default : @default;

			return icon;
		}
	}
}