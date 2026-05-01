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
		private static readonly Dictionary<string, TypeMetadata> METADATA = new();
		private static readonly GUIContent NOT_MANAGED_REFERENCE_CONTENT = new("The property type is not manage reference.");

		public static bool DrawTypeDropdown(Rect position, SerializedProperty property, IVirtualType virtualType)
		{
			if (property.propertyType != SerializedPropertyType.ManagedReference)
			{
				EditorGUI.LabelField(position, NOT_MANAGED_REFERENCE_CONTENT);
				return false;
			}

			var typeMetadata = GetOrCreateMetadata(property.managedReferenceFullTypename);
			var dropdownPosition = new Rect(position)
			{
				width = position.width - EditorGUIUtility.labelWidth,
				x = position.x + EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			if (!EditorGUI.DropdownButton(dropdownPosition, typeMetadata.Content, FocusType.Keyboard))
			{
				return true;
			}

			if (!typeMetadata.IsInitialized)
			{
				var referenceType = TypeUtils.ExtractTypeFromString(property.managedReferenceFieldTypename);
				typeMetadata.Initialize(virtualType, referenceType);
			}

			var dropdown = new AdvancedTypeDropdown(typeMetadata.DerivedTypes, new AdvancedDropdownState(), OnSelected);
			dropdown.Show(dropdownPosition);
			return true;

			void OnSelected(Type type)
			{
				OnItemCreate(type, property, position);
			}
		}

		internal static void ClearCache() => METADATA.Clear();

		internal static long CalculateCacheSizeBytes()
		{
			long total = 0;
			foreach (var (key, value) in METADATA)
			{
				if (key != null)
				{
					total += sizeof(char) * key.Length;
					total += 20;
				}

				total += TypeUtils.EstimateObjectSize(value);
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

		private static TypeMetadata GetOrCreateMetadata(string fullTypename)
		{
			if (METADATA.TryGetValue(fullTypename, out var metadata))
			{
				return metadata;
			}

			var referenceType = TypeUtils.ExtractTypeFromString(fullTypename);
			var content = CreateContent(fullTypename, referenceType);
			metadata = new TypeMetadata(content);
			METADATA.Add(fullTypename, metadata);

			return metadata;
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

		private sealed record TypeMetadata(GUIContent Content)
		{
			public GUIContent Content { get; } = Content;
			public bool IsInitialized => DerivedTypes != null;
			public IEnumerable<Type> DerivedTypes { get; private set; }

			public void Initialize(IVirtualType virtualType, Type type) =>
				DerivedTypes = virtualType.GetDerivedTypes(type);
		}
	}
}