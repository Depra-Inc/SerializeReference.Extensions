// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializeReference.Extensions.Editor.Internal;
using Depra.SerializeReference.Extensions.Editor.Settings;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	public static class SerializeReferenceUtility
	{
		private static readonly char[] SEPARATORS = { '.', '/' };
		private static readonly Dictionary<string, GUIContent> CONTENT_CACHE = new();
		private static readonly Dictionary<string, Type[]> DERIVED_TYPES_CACHE = new();
		private static readonly GUIContent NOT_MANAGED_REFERENCE_CONTENT = new("The property type is not manage reference.");

		public static bool DrawTypeDropdown(Rect position, SerializedProperty property, IVirtualType virtualType)
		{
			if (property.propertyType != SerializedPropertyType.ManagedReference)
			{
				EditorGUI.LabelField(position, NOT_MANAGED_REFERENCE_CONTENT);
				return false;
			}

			var content = GetOrCreateContent(property.managedReferenceFullTypename);
			var dropdownPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (!EditorGUI.DropdownButton(dropdownPosition, content, FocusType.Keyboard))
			{
				return true;
			}

			var fieldTypename = property.managedReferenceFieldTypename;
			if (!DERIVED_TYPES_CACHE.TryGetValue(fieldTypename, out var derivedTypes))
			{
				var referenceType = TypeUtils.ExtractTypeFromString(fieldTypename);
				derivedTypes = virtualType.GetDerivedTypes(referenceType).ToArray();
				DERIVED_TYPES_CACHE.Add(fieldTypename, derivedTypes);
			}

			var dropdown = new AdvancedTypeDropdown(derivedTypes, new AdvancedDropdownState(), OnSelected);
			dropdown.Show(dropdownPosition);
			return true;

			void OnSelected(Type type)
			{
				OnItemCreate(type, property, position);
			}
		}

		internal static void ClearCache()
		{
			CONTENT_CACHE.Clear();
			DERIVED_TYPES_CACHE.Clear();
		}

		internal static long CalculateCacheSizeBytes()
		{
			long total = 0;
			foreach (var (key, value) in CONTENT_CACHE)
			{
				if (key != null)
				{
					total += sizeof(char) * key.Length;
					total += 20;
				}
 
				total += TypeUtils.EstimateObjectSize(value);
			}
 
			foreach (var (key, value) in DERIVED_TYPES_CACHE)
			{
				if (key != null)
				{
					total += sizeof(char) * key.Length;
					total += 20;
				}
 
				total += IntPtr.Size * (value?.Length ?? 0);
			}
 
			return total;
		}

		internal static string[] SplitName(string name) => !string.IsNullOrWhiteSpace(name)
			? name.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries)
			: Array.Empty<string>();

		private static void OnItemCreate(Type type, SerializedProperty property, Rect position)
		{
			if (type?.IsGenericType == true)
			{
				var propertyType = TypeUtils.ExtractTypeFromString(property.managedReferenceFieldTypename);
				var concreteType = TypeUtils.GetConcreteGenericType(propertyType, type);
				if (concreteType != null)
				{
					var instance = type.CreateInstance();
					property.managedReferenceValue = instance;
					property.isExpanded = instance != null;
					property.serializedObject.ApplyModifiedProperties();
				}
				else
				{
					GenericTypeCreateWindow.Open(property, position, type, _ => { });
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

		private static GUIContent GetOrCreateContent(string fullTypename)
		{
			if (string.IsNullOrEmpty(fullTypename))
			{
				return NullDropdownItem.CONTENT;
			}

			if (CONTENT_CACHE.TryGetValue(fullTypename, out var content))
			{
				return content;
			}

			var type = TypeUtils.ExtractTypeFromString(fullTypename);
			content = CreateContent(fullTypename, type);
			CONTENT_CACHE.Add(fullTypename, content);

			return content;
		}

		private static GUIContent CreateContent(string fullTypename, Type type)
		{
			if (string.IsNullOrEmpty(fullTypename))
			{
				return NullDropdownItem.CONTENT;
			}

			if (type.TryGetCustomAttribute<SerializeReferenceMenuPathAttribute>(out var dropdownNameAttribute))
			{
				return new GUIContent(SplitName(dropdownNameAttribute.Path)[^1]);
			}

			if (type.IsGenericType)
			{
				var genericNames = type.GenericTypeArguments.Select(t => t.Name);
				var genericParamNames = " [" + string.Join(",", genericNames) + "]";
				var genericName = ObjectNames.NicifyVariableName(type.Name) + genericParamNames;

				return new GUIContent(genericName);
			}

			if (type.IsNested)
			{
				var typeName = type.FullName;
				var lastDot = typeName?.LastIndexOf('.');
				if (lastDot > 0)
				{
					typeName = typeName.Substring(lastDot.Value + 1);
				}

				return new GUIContent(ObjectNames.NicifyVariableName(typeName));
			}

			var fallbackTypeName = SplitName(type.FullName)[^1];
			fallbackTypeName = ObjectNames.NicifyVariableName(fallbackTypeName);
			var contentIcon = SerializeReferenceSettings.instance.GetIcon(type);

			return new GUIContent(fallbackTypeName, contentIcon);
		}
	}
}