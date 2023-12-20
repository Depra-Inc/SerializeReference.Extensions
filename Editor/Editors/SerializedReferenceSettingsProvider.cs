// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor;

namespace Depra.Inspector.SerializedReference.Editor.Editors
{
	public sealed class SerializedReferenceSettingsProvider : SettingsProvider
	{
		private static readonly string PATH = "Project/" + ObjectNames.NicifyVariableName(nameof(SerializedReference));

		public SerializedReferenceSettingsProvider() : base(PATH, SettingsScope.Project) { }

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