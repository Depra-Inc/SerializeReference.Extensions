// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	[FilePath(nameof(SerializeReferenceSettings), FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class SerializeReferenceSettings : ScriptableSingleton<SerializeReferenceSettings>
	{
		[SerializeField] private SearchType _metadataSearchType = SearchType.ATTRIBUTE;
		[SerializeField] private string _defaultIconName = "cs Script Icon";

		public void Save() => Save(true);

		public Texture2D GetIcon(Type type)
		{
			var defaultIcon = EditorIcons.GetIcon(_defaultIconName).image as Texture2D;
			return _metadataSearchType switch
			{
				SearchType.OFF => defaultIcon,
				SearchType.ATTRIBUTE => GetIconFromAttribute(type, defaultIcon),
				SearchType.SCRIPT_IMPORTER => ScriptImporter.GetIcon(type),
				_ => defaultIcon
			};
		}

		private Texture2D GetIconFromAttribute(Type type, Texture2D defaultIcon)
		{
			if (type.TryGetCustomAttribute<SerializeReferenceIconAttribute>(out var iconInfo) == false ||
			    string.IsNullOrEmpty(iconInfo.Name))
			{
				return defaultIcon;
			}

			return EditorIcons.GetIcon(iconInfo.Name).image as Texture2D;
		}

		private enum SearchType
		{
			[InspectorName("Off")] OFF,
			[InspectorName("Attribute")] ATTRIBUTE,
			[InspectorName("Script Importer")] SCRIPT_IMPORTER
		}
	}
}