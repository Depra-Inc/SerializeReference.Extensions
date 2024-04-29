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

		public static Texture2D GetIcon(Type type)
		{
			const string SEARCH_PATTERN = "{0} t:script";
			var guids = AssetDatabase.FindAssets(string.Format(SEARCH_PATTERN, type.Name));
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var importer = AssetImporter.GetAtPath(path);
				if (importer is MonoImporter monoImporter)
				{
					return monoImporter.GetIcon() ?? DEFAULT_ICON;
				}
			}

			return DEFAULT_ICON;
		}
	}
}