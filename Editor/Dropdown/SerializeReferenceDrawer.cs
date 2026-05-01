// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Depra.SerializeReference.Extensions.Editor.Internal;
using Depra.SerializeReference.Extensions.Editor.Settings;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	[CustomPropertyDrawer(typeof(SerializeReferenceAttribute))]
	public sealed class SerializeReferenceDrawer : PropertyDrawer
	{
		internal static readonly char[] SEPARATORS = { '.', '/' };
		private static readonly GUIContent NOT_MANAGED_REFERENCE_CONTENT =
			new("The property type is not manage reference.");

		private static readonly Dictionary<string, GUIContent> CONTENT_CACHE = new();

		public static void ClearCache() => CONTENT_CACHE.Clear();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();

			if (property.propertyType == SerializedPropertyType.ManagedReference)
			{
				DrawManagedReferenceGUI(position, property);
				EditorGUI.PropertyField(position, property, label, true);
			}
			else
			{
				EditorGUI.LabelField(position, label, NOT_MANAGED_REFERENCE_CONTENT);
			}

			EditorGUI.EndProperty();
			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property) => true;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);

		private void DrawManagedReferenceGUI(Rect position, SerializedProperty property)
		{
			var dropdownContent = GetTypeContent(property);
			var dropdownPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (!EditorGUI.DropdownButton(dropdownPosition, dropdownContent, FocusType.Keyboard))
			{
				return;
			}

			var dropdown = CreateTypeDropdown(property, position);
			dropdown.Show(dropdownPosition);
		}

		private AdvancedTypeDropdown CreateTypeDropdown(SerializedProperty property, Rect position)
		{
			var referenceType = TypeExtensions.ExtractTypeFromString(property.managedReferenceFieldTypename);
			var referenceAttribute = fieldInfo.GetCustomAttribute<SerializeReferenceAttribute>();
			var derivedTypes = referenceAttribute.GetDerivedTypes(referenceType);
			return new AdvancedTypeDropdown(derivedTypes, new AdvancedDropdownState(),
				type => OnItemCreate(type, property, position));
		}

		private void OnItemCreate(Type type, SerializedProperty property, Rect position)
		{
			if (type?.IsGenericType == true)
			{
				var propertyType = TypeExtensions.ExtractTypeFromString(property.managedReferenceFieldTypename);
				var concreteType = TypeExtensions.GetConcreteGenericType(propertyType, type);
				if (concreteType != null)
				{
					var instance = type.CreateInstance();
					property.managedReferenceValue = instance;
					property.isExpanded = instance != null;
					property.serializedObject.ApplyModifiedProperties();
				}
				else
				{
					GenericTypeCreateWindow.Open(property, position, type, selectedType => { });
				}
			}
			else
			{
				var instance = type?.CreateInstance();
				property.managedReferenceValue = instance;
				property.isExpanded = instance != null;
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private GUIContent GetTypeContent(SerializedProperty property)
		{
			var fullTypename = property.managedReferenceFullTypename;
			if (string.IsNullOrEmpty(fullTypename))
			{
				return NullDropdownItem.CONTENT;
			}

			if (CONTENT_CACHE.TryGetValue(fullTypename, out var cachedTypename))
			{
				return cachedTypename;
			}

			if (property.propertyType != SerializedPropertyType.ManagedReference)
			{
				Debug.LogException(new SerializedPropertyTypeMustBeManagedReference(nameof(property)));
				return NOT_MANAGED_REFERENCE_CONTENT;
			}

			var type = TypeExtensions.ExtractTypeFromString(property.managedReferenceFullTypename);
			if (type.TryGetCustomAttribute<SerializeReferenceMenuPathAttribute>(out var dropdownNameAttribute))
			{
				var splitTypeName = MenuPath.SplitName(dropdownNameAttribute.Path, SEPARATORS);
				var customContent = new GUIContent(splitTypeName[^1]);
				CONTENT_CACHE.Add(fullTypename, customContent);

				return customContent;
			}

			if (type.IsGenericType)
			{
				var genericNames = type.GenericTypeArguments.Select(t => t.Name);
				var genericParamNames = " [" + string.Join(",", genericNames) + "]";
				var genericName = ObjectNames.NicifyVariableName(type.Name) + genericParamNames;
				var genericContent = new GUIContent(genericName);
				CONTENT_CACHE.Add(fullTypename, genericContent);

				return genericContent;
			}

			if (type.IsNested)
			{
				var typeName = type.FullName;
				var lastDot = typeName?.LastIndexOf('.');
				if (lastDot > 0)
				{
					typeName = typeName.Substring(lastDot.Value + 1);
				}

				var nestedContent = new GUIContent(ObjectNames.NicifyVariableName(typeName));
				CONTENT_CACHE.Add(fullTypename, nestedContent);

				return nestedContent;
			}

			var fallbackTypeName = MenuPath.SplitName(type.FullName, SEPARATORS)[^1];
			fallbackTypeName = ObjectNames.NicifyVariableName(fallbackTypeName);
			var contentIcon = SerializeReferenceSettings.instance.GetIcon(type);
			var content = new GUIContent(fallbackTypeName, contentIcon);
			CONTENT_CACHE.Add(fullTypename, content);

			return content;
		}

		private sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
		{
			public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(
				$"The serialized property type must be {nameof(SerializedPropertyType.ManagedReference)}",
				paramName) { }
		}
	}
}