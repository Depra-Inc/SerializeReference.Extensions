// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializedReference.Dropdown.Editor.Popup;
using Depra.SerializedReference.Dropdown.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Depra.SerializedReference.Dropdown.Editor.Extensions
{
	internal static class AdvancedDropdownExtensions
	{
		public static void Populate(this AdvancedDropdownItem root, IEnumerable<Type> types)
		{
			var itemCount = 0;
			AddNullTypeItem(root, ref itemCount);

			var allTypes = types.OrderByType();
			foreach (var type in allTypes)
			{
				AddTypeToHierarchy(root, type, ref itemCount);
			}
		}

		private static void AddNullTypeItem(AdvancedDropdownItem root, ref int itemCount) =>
			root.AddChild(new AdvancedTypePopupItem(null, SubtypeMenuAliasAttribute.NULL_DISPLAY_NAME)
				{ id = itemCount++ });

		private static void AddTypeToHierarchy(AdvancedDropdownItem root, Type type, ref int itemCount)
		{
			var splitTypePath = type.SplitTypePath();
			if (splitTypePath.Length == 0)
			{
				return;
			}

			var parent = root;
			foreach (var namespaceName in splitTypePath.Take(splitTypePath.Length - 1))
			{
				parent = FindOrCreateChildItem(parent, namespaceName, ref itemCount);
			}

			var typeName = ObjectNames.NicifyVariableName(splitTypePath.Last());
			var item = new AdvancedTypePopupItem(type, typeName)
			{
				id = itemCount++
			};
			parent.AddChild(item);
		}

		private static AdvancedDropdownItem FindOrCreateChildItem(AdvancedDropdownItem parent, string itemName,
			ref int itemCount)
		{
			var foundItem = parent.children.FirstOrDefault(item => item.name == itemName);
			if (foundItem != null)
			{
				return foundItem;
			}

			var newItem = new AdvancedDropdownItem(itemName) { id = itemCount++, };
			parent.AddChild(newItem);

			return newItem;
		}
	}
}