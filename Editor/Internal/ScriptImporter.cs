// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class ScriptImporter
	{
		public static Texture2D GetIcon(Type type) => GetIcon(type, EditorIcons.SCRIPT_ICON);

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