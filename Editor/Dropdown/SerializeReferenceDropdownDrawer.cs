// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Module = Depra.SerializeReference.Extensions.Editor.Internal.Module;
using Object = UnityEngine.Object;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	[CustomPropertyDrawer(typeof(SerializeReferenceDropdownAttribute))]
	internal sealed class SerializeReferenceDropdownDrawer : PropertyDrawer
	{
		private const int MAX_LINE_COUNT = 13;

		private readonly Dictionary<string, GUIContent> _typeNameCache = new();
		private readonly Dictionary<long, AdvancedTypeDropdown> _dropdowns = new();

		private SerializedProperty _targetProperty;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();

			if (property.propertyType == SerializedPropertyType.ManagedReference)
			{
				DrawManagedReferenceGUI(position, property, label);
			}
			else
			{
				var content = new GUIContent("The property type is not manage reference.");
				EditorGUI.LabelField(position, label, content);
			}

			EditorGUI.EndProperty();
			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property) => true;

		private void DrawManagedReferenceGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var dropdown = _dropdowns.TryGetValue(property.managedReferenceId, out var created)
				? created
				: CreateTypeDropdown(property);

			var dropdownPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (EditorGUI.DropdownButton(dropdownPosition, GetTypeContent(property), FocusType.Keyboard))
			{
				_targetProperty = property;
				dropdown.Show(dropdownPosition);
			}

			EditorGUI.PropertyField(position, property, label, true);
		}

		private AdvancedTypeDropdown CreateTypeDropdown(SerializedProperty property)
		{
			var referenceType = property.propertyType == SerializedPropertyType.ManagedReference
				? GetType(property.managedReferenceFieldTypename)
				: throw new SerializedPropertyTypeMustBeManagedReference(nameof(property));

			var derivedTypes = TypeCache
				.GetTypesDerivedFrom(referenceType)
				.Where(x => (x.IsPublic || x.IsNestedPublic) &&
				            x.IsAbstract == false &&
				            x.IsGenericType == false &&
				            typeof(Object).IsAssignableFrom(x) == false);

			var dropdown = new AdvancedTypeDropdown(derivedTypes, MAX_LINE_COUNT, new AdvancedDropdownState());
			dropdown.OnItemSelected += OnItemCreated;

			_dropdowns.Add(property.managedReferenceId, dropdown);

			return dropdown;
		}

		private void OnItemCreated(AdvancedDropdownItem item)
		{
			var instance = item is TypeDropdownItem typeItem
				? typeItem.Type != null ? Activator.CreateInstance(typeItem.Type) : null
				: null;

			_targetProperty.managedReferenceValue = instance;
			_targetProperty.isExpanded = instance != null;
			_targetProperty.serializedObject.ApplyModifiedProperties();
		}

		private GUIContent GetTypeContent(SerializedProperty property)
		{
			var fullTypename = property.managedReferenceFullTypename;
			if (string.IsNullOrEmpty(fullTypename))
			{
				return new GUIContent(NullDropdownItem.DISPLAY_NAME, EditorIcons.NULL_ICON.image);
			}

			if (_typeNameCache.TryGetValue(fullTypename, out var cachedTypename))
			{
				return cachedTypename;
			}

			var type = property.propertyType == SerializedPropertyType.ManagedReference
				? GetType(property.managedReferenceFullTypename)
				: throw new SerializedPropertyTypeMustBeManagedReference(nameof(property));

			var splitTypeName = type.TryGetCustomAttribute(out SerializeReferenceMenuPathAttribute subtypeAlias)
				? MenuPath.SplitName(subtypeAlias.Path, Module.SEPARATORS)
				: MenuPath.SplitName(type.FullName, Module.SEPARATORS);

			var typeName = splitTypeName[^1];
			typeName = ObjectNames.NicifyVariableName(typeName);
			var contentIcon = ScriptImporter.GetIcon(type, EditorIcons.DROPDOWN_ICON.image);
			var content = new GUIContent(typeName, contentIcon);
			_typeNameCache.Add(fullTypename, content);

			return content;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);

		private Type GetType(string typeName)
		{
			var splitIndex = typeName.IndexOf(' ');
			var assembly = Assembly.Load(typeName[..splitIndex]);
			var type = assembly.GetType(typeName[(splitIndex + 1)..]);

			return type;
		}

		private sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
		{
			public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(
				$"The serialized property type must be {nameof(SerializedPropertyType.ManagedReference)}", paramName) { }
		}
	}
}