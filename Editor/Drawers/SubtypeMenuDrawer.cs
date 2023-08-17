// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using Depra.SerializeReference.Selection.Editor.Extensions;
using Depra.SerializeReference.Selection.Editor.Popup;
using Depra.SerializeReference.Selection.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Selection.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(SubtypeMenuAttribute))]
	internal sealed class SubtypeMenuDrawer : PropertyDrawer
	{
		private const int MAX_TYPE_POPUP_LINE_COUNT = 13;
		private static readonly GUIContent NULL_DISPLAY_NAME = new(TypeExtensions.NULL_DISPLAY_NAME);
		private static readonly GUIContent IS_NOT_MANAGED_REFERENCE_LABEL =
			new("The property type is not manage reference.");

		private readonly Dictionary<string, TypePopupCache> _typePopups = new();
		private readonly Dictionary<string, GUIContent> _typeNameCaches = new();

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
			var popup = GetTypePopup(property);
			var popupPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (EditorGUI.DropdownButton(popupPosition, GetTypeName(property), FocusType.Keyboard))
			{
				_targetProperty = property;
				popup.TypePopup.Show(popupPosition);
			}

			EditorGUI.PropertyField(position, property, label, true);
		}

		private TypePopupCache GetTypePopup(SerializedProperty property)
		{
			if (_typePopups.TryGetValue(property.managedReferenceFieldTypename, out var result))
			{
				return result;
			}

			var state = new AdvancedDropdownState();
			var baseType = property.GetManagedReferenceFieldType();
			var types = baseType.GetDerivedTypes();
			var popup = new AdvancedTypePopup(types, MAX_TYPE_POPUP_LINE_COUNT, state);

			popup.OnItemSelected += OnItemCreated;

			_typePopups.Add(property.managedReferenceFieldTypename, new TypePopupCache(popup, state));

			return result;

			void OnItemCreated(AdvancedTypePopupItem item)
			{
				var managedReference = _targetProperty.SetManagedReference(item.Type);
				_targetProperty.isExpanded = managedReference != null;
				_targetProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		private GUIContent GetTypeName(SerializedProperty property)
		{
			if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
			{
				return NULL_DISPLAY_NAME;
			}

			if (_typeNameCaches.TryGetValue(property.managedReferenceFullTypename, out var cachedTypeName))
			{
				return cachedTypeName;
			}

			var type = property.GetManagedReferenceType();
			if (TryGetTypeNameWithoutPath(type, out var typeName) == false)
			{
				if (string.IsNullOrEmpty(typeName))
				{
					typeName = ObjectNames.NicifyVariableName(type.Name);
				}
			}

			var content = new GUIContent(typeName);
			_typeNameCaches.Add(property.managedReferenceFullTypename, content);

			return content;
		}

		private static bool TryGetTypeNameWithoutPath(Type type, out string typeName)
		{
			var typeMenu = type.GetTypeMenuAliasAttribute();
			if (typeMenu == null)
			{
				typeName = string.Empty;
				return false;
			}

			typeName = typeMenu.GetTypeNameWithoutPath();
			if (string.IsNullOrWhiteSpace(typeName))
			{
				return false;
			}

			typeName = ObjectNames.NicifyVariableName(typeName);
			return true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			EditorGUI.GetPropertyHeight(property, true);

		private readonly struct TypePopupCache
		{
			public readonly AdvancedTypePopup TypePopup;
			public readonly AdvancedDropdownState State;

			public TypePopupCache(AdvancedTypePopup typePopup, AdvancedDropdownState state)
			{
				TypePopup = typePopup;
				State = state;
			}
		}
	}
}