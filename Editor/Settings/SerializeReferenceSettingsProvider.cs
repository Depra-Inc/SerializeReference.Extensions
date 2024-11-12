// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	[Serializable]
	internal sealed class SerializeReferenceSettingsProvider : SettingsProvider
	{
		private static readonly string PATH = nameof(Editor) + "/" +
		                                      ObjectNames.NicifyVariableName(nameof(SerializeReferenceAttribute));

		internal static SerializeReferenceSettingsProvider Instance { get; private set; }

		public static bool IsAvailable() => SerializeReferenceSettings.instance != null;

		[SettingsProvider]
		public static SettingsProvider CreateTriInspectorSettingsProvider() => Instance =
			new SerializeReferenceSettingsProvider(PATH, SettingsScope.Project)
			{
				keywords = GetSearchKeywordsFromGUIContentProperties<Styles>(),
			};

		private SerializedObject _serializedObject;

		private SerializeReferenceSettingsProvider(string path, SettingsScope scope) : base(path, scope) { }

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			_serializedObject = new SerializedObject(SerializeReferenceSettings.instance);
			var niceName = ObjectNames.NicifyVariableName(nameof(SerializeReferenceSettings));
			var title = new Label { text = niceName }.SetHeaderStyle();
			title.AddToClassList("title");
			rootElement.Add(title);

			var properties = new VisualElement().SetPropertiesStyle();
			properties.AddToClassList("property-list");
			rootElement.Add(properties);

			var iconSearchType = new PropertyField(_serializedObject.FindProperty("_metadataSearchType"));
			iconSearchType.RegisterValueChangeCallback(_ => SerializeReferenceSettings.instance.Save());
			properties.Add(iconSearchType);

			var defaultIconName = new PropertyField(_serializedObject.FindProperty("_defaultIconName"));
			defaultIconName.RegisterValueChangeCallback(_ => SerializeReferenceSettings.instance.Save());
			properties.Add(defaultIconName);

			rootElement.Bind(_serializedObject);
		}

		private sealed class Styles { }
	}
}