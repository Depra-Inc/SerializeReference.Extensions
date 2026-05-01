// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	[Serializable]
	internal sealed class SerializeReferenceSettingsProvider : SettingsProvider
	{
		private const string PATH = "Editor/Serialize References";

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
			var title = new Label { text = niceName };
			SetHeaderStyle(title);
			title.AddToClassList("title");
			rootElement.Add(title);

			var properties = new VisualElement();
			SetPropertiesStyle(properties);
			properties.AddToClassList("property-list");
			rootElement.Add(properties);

			var iconSearchType = new PropertyField(_serializedObject.FindProperty("_metadataSearchType"));
			iconSearchType.RegisterValueChangeCallback(_ => SerializeReferenceSettings.instance.Save());
			properties.Add(iconSearchType);

			var sortToggle = new PropertyField(_serializedObject.FindProperty("_sortByAttribute"));
			sortToggle.RegisterValueChangeCallback(_ => SerializeReferenceSettings.instance.Save());
			properties.Add(sortToggle);

			var genericToggle = new PropertyField(_serializedObject.FindProperty("_serializeGenericTypes"));
			genericToggle.SetEnabled(false);
			properties.Add(genericToggle);

			properties.Add(new HelpBox(
				"This functionality is not available in the current version.",
				HelpBoxMessageType.Info));

			properties.Add(new Button(SerializeReferenceSettings.ClearCache) { text = "Clear Cache" });
			rootElement.Bind(_serializedObject);
		}

		private VisualElement SetPropertiesStyle(VisualElement self)
		{
			self.style.marginTop = 9;
			self.style.marginLeft = 9;

			return self;
		}

		private Label SetHeaderStyle(Label self)
		{
			self.style.fontSize = 19;
			self.style.marginTop = 1;
			self.style.marginLeft = 9;
			self.style.unityFontStyleAndWeight = FontStyle.Bold;

			return self;
		}

		private sealed class Styles { }
	}
}