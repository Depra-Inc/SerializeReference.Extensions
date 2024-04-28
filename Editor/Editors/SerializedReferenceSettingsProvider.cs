// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;
using static Depra.SerializeReference.Extensions.Editor.Menu.Module;

namespace Depra.SerializeReference.Extensions.Editor.Editors
{
	public sealed class SerializedReferenceSettingsProvider : SettingsProvider
	{
		private SerializedReferenceSettingsProvider() : base(
			path: "Project/" + ObjectNames.NicifyVariableName(nameof(SerializeReference) + SLASH + nameof(Dropdown)),
			scopes: SettingsScope.Project) { }

		public override void OnGUI(string searchContext)
		{
			EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
			base.OnGUI(searchContext);
			EditorGUI.EndDisabledGroup();
		}

		[SettingsProvider]
		public static SettingsProvider CreateTriInspectorSettingsProvider() => new SerializedReferenceSettingsProvider
		{
			keywords = GetSearchKeywordsFromGUIContentProperties<Styles>(),
		};

		private sealed class Styles { }
	}
}