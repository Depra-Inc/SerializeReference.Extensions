// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class ScriptImporter
	{
		private static readonly Texture2D DEFAULT_ICON = (Texture2D) EditorIcons.SCRIPT_ICON.image;

		public static Texture2D GetIcon(Type type) => GetIcon(type, DEFAULT_ICON);

		public static Texture2D GetIcon(Type type, Texture @default) => GetIcon(type, (Texture2D) @default);

		public static Texture2D GetIcon(Type type, Texture2D @default)
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