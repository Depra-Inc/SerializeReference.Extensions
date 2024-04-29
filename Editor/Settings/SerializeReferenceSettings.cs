// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	[FilePath(nameof(SerializeReferenceSettings), FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class SerializeReferenceSettings : ScriptableSingleton<SerializeReferenceSettings>
	{
		[SerializeField] private bool _searchCustomIcons;

		public bool SearchCustomIcons => _searchCustomIcons;

		public void Save() => Save(true);
	}
}