// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	/// <summary>
	/// A type popup with a fuzzy finder.
	/// </summary>
	internal sealed class AdvancedTypeDropdown : AdvancedDropdown
	{
		private const int MAX_LINE_COUNT = 13;
		private readonly IEnumerable<Type> _types;
		private readonly IEnumerable<string> _typeNames;
		private readonly Action<AdvancedDropdownItem> _onTypeSelected;

		public AdvancedTypeDropdown(IEnumerable<Type> types, AdvancedDropdownState state, 
			Action<AdvancedDropdownItem> onSelected, int maxLineCount = MAX_LINE_COUNT) : base(state)
		{
			_types = types;
			_onTypeSelected = onSelected;
			var headerHeight = EditorGUIUtility.singleLineHeight * 2f;
			minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + headerHeight);
		}

		public AdvancedTypeDropdown(IEnumerable<string> typeNames, AdvancedDropdownState state,
			Action<AdvancedDropdownItem> onSelected, int maxLineCount = MAX_LINE_COUNT) : base(state)
		{
			_typeNames = typeNames;
			_onTypeSelected = onSelected;
			var headerHeight = EditorGUIUtility.singleLineHeight * 2f;
			minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + headerHeight);
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var itemCount = 1;
			var root = new AdvancedDropdownItem("Select Type");
			root.AddChild(new NullDropdownItem { id = itemCount++ });

			if (_typeNames != null)
			{
				foreach (var typeName in _typeNames.OrderBy(n => n))
				{
					var item = new AdvancedDropdownItem(typeName) { id = itemCount++ };
					root.AddChild(item);
				}

				return root;
			}

			foreach (var type in OrderByAttribute(_types))
			{
				var splitPath = type.TryGetCustomAttribute(out SerializeReferenceMenuPathAttribute menuPathMeta)
					? MenuPath.SplitName(menuPathMeta.Path, Module.SEPARATORS)
					: MenuPath.SplitName(type.FullName, Module.SEPARATORS);

				if (type.IsGenericType)
				{
					var genericNames = type.GenericTypeArguments.Select(t => t.Name);
					var genericParamNames = " [" + string.Join(",", genericNames) + "]";
					var genericName = ObjectNames.NicifyVariableName(type.Name) + genericParamNames;
					splitPath[^1] = genericName;
				}

				if (type.IsNested)
				{
					splitPath[^1] = splitPath[^1].Replace("+", ".");
				}

				AddTypeToHierarchy(root, type, splitPath, ref itemCount);
			}

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item) => _onTypeSelected?.Invoke(item);

		private IEnumerable<Type> OrderByAttribute(IEnumerable<Type> self) => self.OrderBy(type =>
			type?.GetCustomAttribute<SerializeReferenceOrderAttribute>()?.Order ?? 0);

		private void AddTypeToHierarchy(AdvancedDropdownItem root, Type type, string[] names, ref int count)
		{
			var parent = root;
			var parentTypeNames = names[..^1];
			foreach (var namespaceName in parentTypeNames)
			{
				parent = FindOrCreateChild(parent, namespaceName, ref count);
			}

			var typeName = ObjectNames.NicifyVariableName(names[^1]);
			var item = new TypeDropdownItem(type, typeName) { id = count++ };
			parent.AddChild(item);
		}

		private AdvancedDropdownItem FindOrCreateChild(AdvancedDropdownItem parent, string name, ref int count)
		{
			foreach (var child in parent.children)
			{
				if (child.name == name)
				{
					return child;
				}
			}

			var item = new AdvancedDropdownItem(name) { id = count++, };
			parent.AddChild(item);

			return item;
		}

		internal delegate void SelectionHandler(AdvancedDropdownItem item);
	}
}