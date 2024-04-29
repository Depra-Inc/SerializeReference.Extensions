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
			const string SEARCH_PATTERN = "{0} t:script";
			var guids = AssetDatabase.FindAssets(string.Format(SEARCH_PATTERN, type.Name));
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var importer = AssetImporter.GetAtPath(path);
				if (importer is not MonoImporter monoImporter)
				{
					continue;
				}

				return monoImporter.GetIcon() ?? @default;
			}

			return @default;
		}
	}
}