// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	/// <summary>
	/// A type popup with a fuzzy finder.
	/// </summary>
	internal sealed class AdvancedTypeDropdown : AdvancedDropdown
	{
		private const string DROPDOWN_NAME = "Select Type";
		private static readonly float HEADER_HEIGHT = EditorGUIUtility.singleLineHeight * 2f;

		private readonly IEnumerable<Type> _types;

		public event Action<AdvancedTypeDropdownItem> OnItemSelected;

		public AdvancedTypeDropdown(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) : base(state)
		{
			_types = types;
			minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + HEADER_HEIGHT);
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var itemCount = 1;
			var root = new AdvancedDropdownItem(DROPDOWN_NAME);
			NullAdvancedDropdownItem.AddAsChild(root, itemCount);

			foreach (var type in OrderAttribute.OrderBy(_types))
			{
				var splitPath = type.TryGetCustomAttribute(out SubtypeAliasAttribute subtypeAlias)
					? subtypeAlias.Alias.SplitDropdownName(Separators.ALL)
					: type.FullName.SplitDropdownName(Separators.ALL);

				if (type.IsNested)
				{
					splitPath[^1] = splitPath[^1].Replace("+", ".");
				}

				AddTypeToHierarchy(root, type, splitPath, ref itemCount);
			}

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item)
		{
			if (item is AdvancedTypeDropdownItem dropdownItem)
			{
				OnItemSelected?.Invoke(dropdownItem);
			}
		}

		private static void AddTypeToHierarchy(AdvancedDropdownItem self, Type type, string[] typeNames,
			ref int itemCount)
		{
			var parent = self;
			foreach (var namespaceName in typeNames)
			{
				parent = FindOrCreateChildItem(parent, namespaceName, ref itemCount);
			}

			var typeName = ObjectNames.NicifyVariableName(typeNames[^1]);
			var item = new AdvancedTypeDropdownItem(type, typeName) { id = itemCount++ };
			parent.AddChild(item);
		}

		private static AdvancedDropdownItem FindOrCreateChildItem(AdvancedDropdownItem self, string itemName,
			ref int itemCount)
		{
			foreach (var child in self.children)
			{
				if (child.name == itemName)
				{
					return child;
				}
			}

			var newItem = new AdvancedDropdownItem(itemName) { id = itemCount++, };
			self.AddChild(newItem);

			return newItem;
		}
	}
}