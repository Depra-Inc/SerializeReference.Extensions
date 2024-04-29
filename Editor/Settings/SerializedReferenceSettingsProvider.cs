// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	public sealed class SerializedReferenceSettingsProvider : SettingsProvider
	{
		internal static SerializedReferenceSettingsProvider Instance { get; private set; }

		private SerializedReferenceSettingsProvider() : base(
			path: "Project/" + ObjectNames.NicifyVariableName(nameof(SerializeReference)),
			scopes: SettingsScope.Project) { }

		public override void OnGUI(string searchContext)
		{
			EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
			base.OnGUI(searchContext);
			EditorGUI.EndDisabledGroup();
		}

		[SettingsProvider]
		public static SettingsProvider CreateTriInspectorSettingsProvider() => Instance =
			new SerializedReferenceSettingsProvider
			{
				keywords = GetSearchKeywordsFromGUIContentProperties<Styles>(),
			};

		private sealed class Styles { }
	}
}