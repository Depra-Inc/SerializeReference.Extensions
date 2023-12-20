// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	[CustomPropertyDrawer(typeof(SubtypeDropdownAttribute))]
	internal sealed class SubtypeDropdownDrawer : PropertyDrawer
	{
		private const int MAX_LINE_COUNT = 13;
		private static readonly GUIContent NULL_DISPLAY_NAME = new(NullAdvancedDropdownItem.DISPLAY_NAME);

		private static readonly GUIContent IS_NOT_MANAGED_REFERENCE_LABEL =
			new("The property type is not manage reference.");

		private readonly Dictionary<string, GUIContent> _typeNameCaches = new();
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
				EditorGUI.LabelField(position, label, IS_NOT_MANAGED_REFERENCE_LABEL);
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
			var dropdown = CreateTypeDropdown(property);
			var dropdownPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (EditorGUI.DropdownButton(dropdownPosition, GetTypeName(property), FocusType.Keyboard))
			{
				_targetProperty = property;
				dropdown.Show(dropdownPosition);
			}

			EditorGUI.PropertyField(position, property, label, true);
		}

		private AdvancedTypeDropdown CreateTypeDropdown(SerializedProperty property)
		{
			if (_dropdowns.TryGetValue(property.managedReferenceId, out var dropdown))
			{
				return dropdown;
			}

			var referenceType = property.propertyType == SerializedPropertyType.ManagedReference
				? GetType(property.managedReferenceFieldTypename)
				: throw new SerializedPropertyTypeMustBeManagedReference(nameof(property));

			var derivedTypes = TypeCache
				.GetTypesDerivedFrom(referenceType)
				.Where(x => (x.IsPublic || x.IsNestedPublic) &&
				            x.IsAbstract == false &&
				            x.IsGenericType == false &&
				            typeof(Object).IsAssignableFrom(x) == false);

			dropdown = new AdvancedTypeDropdown(derivedTypes, MAX_LINE_COUNT, new AdvancedDropdownState());
			dropdown.OnItemSelected += OnItemCreated;

			_dropdowns.Add(property.managedReferenceId, dropdown);

			return dropdown;
		}

		private void OnItemCreated(AdvancedTypeDropdownItem item)
		{
			var instance = item.Type != null ? Activator.CreateInstance(item.Type) : null;
			_targetProperty.managedReferenceValue = instance;
			_targetProperty.isExpanded = instance != null;
			_targetProperty.serializedObject.ApplyModifiedProperties();
		}

		private GUIContent GetTypeName(SerializedProperty property)
		{
			var fulTypename = property.managedReferenceFullTypename;
			if (string.IsNullOrEmpty(fulTypename))
			{
				return NULL_DISPLAY_NAME;
			}

			if (_typeNameCaches.TryGetValue(fulTypename, out var cachedTypename))
			{
				return cachedTypename;
			}

			var type = property.propertyType == SerializedPropertyType.ManagedReference
				? GetType(property.managedReferenceFullTypename)
				: throw new SerializedPropertyTypeMustBeManagedReference(nameof(property));

			var splitTypeName = type.TryGetCustomAttribute(out SubtypeAliasAttribute subtypeAlias)
				? subtypeAlias.Alias.SplitDropdownName(Separators.ALL)
				: type.FullName.SplitDropdownName(Separators.ALL);

			var typeName = splitTypeName[^1];
			typeName = ObjectNames.NicifyVariableName(typeName);

			var content = new GUIContent(typeName);
			_typeNameCaches.Add(fulTypename, content);

			return content;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);

		private static Type GetType(string typeName)
		{
			var splitIndex = typeName.IndexOf(' ');
			var assembly = Assembly.Load(typeName[..splitIndex]);

			return assembly.GetType(typeName[(splitIndex + 1)..]);
		}
	}
}