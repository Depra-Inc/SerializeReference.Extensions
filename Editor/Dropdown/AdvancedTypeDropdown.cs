// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.Inspector.SerializedReference.Editor.Extensions;
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

		private readonly Type[] _types;

		public event Action<AdvancedTypeDropdownItem> OnItemSelected;

		public AdvancedTypeDropdown(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) : base(state)
		{
			_types = types.ToArray();
			minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + HEADER_HEIGHT);
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var root = new AdvancedDropdownItem(DROPDOWN_NAME);
			root.Populate(_types);

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item)
		{
			if (item is AdvancedTypeDropdownItem typePopupItem)
			{
				OnItemSelected?.Invoke(typePopupItem);
			}
		}
	}
}