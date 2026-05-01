// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using Depra.SerializeReference.Extensions.Editor.Dropdown;
using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	[FilePath(nameof(SerializeReferenceSettings), FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class SerializeReferenceSettings : ScriptableSingleton<SerializeReferenceSettings>
	{
		[SerializeField] private SearchType _metadataSearchType = SearchType.ATTRIBUTE;
		[SerializeField] private bool _sortByAttribute;
		[SerializeField] private bool _serializeGenericTypes;

		private const string DEFAULT_ICON = "cs Script Icon";

		public static void ClearCache() => SerializeReferenceUtility.ClearCache();

		public bool SortByAttribute => _sortByAttribute;
		public bool SerializeGenericTypes => _serializeGenericTypes;

		public void Save() => Save(true);

		public Texture2D GetIcon(Type type)
		{
			var defaultIcon = EditorIcons.GetIcon(DEFAULT_ICON);
			return _metadataSearchType switch
			{
				SearchType.OFF => defaultIcon,
				SearchType.ATTRIBUTE => GetIconFromAttribute(type, defaultIcon),
				SearchType.SCRIPT_IMPORTER => ScriptImporter.GetIcon(type),
				_ => defaultIcon
			};
		}

		private static Texture2D GetIconFromAttribute(Type type, Texture2D defaultIcon)
		{
			if (type.TryGetCustomAttribute<SerializeReferenceIconAttribute>(out var iconInfo) &&
			    !string.IsNullOrEmpty(iconInfo.Name))
			{
				return EditorIcons.GetIcon(iconInfo.Name);
			}

			return defaultIcon;
		}

		private enum SearchType
		{
			[InspectorName("Off")] OFF,
			[InspectorName("Attribute")] ATTRIBUTE,
			[InspectorName("Script Importer")] SCRIPT_IMPORTER
		}
	}
}