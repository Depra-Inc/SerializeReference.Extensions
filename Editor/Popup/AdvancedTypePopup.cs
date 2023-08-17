using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializeReference.Selection.Editor.Extensions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Selection.Editor.Popup
{
	/// <summary>
	/// A type popup with a fuzzy finder.
	/// </summary>
	internal sealed class AdvancedTypePopup : AdvancedDropdown
	{
		private const string DROPDOWN_NAME = "Select Type";
		private static readonly float HEADER_HEIGHT = EditorGUIUtility.singleLineHeight * 2f;

		private readonly Type[] _types;

		public event Action<AdvancedTypePopupItem> OnItemSelected;

		public AdvancedTypePopup(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) : base(state)
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
			if (item is AdvancedTypePopupItem typePopupItem)
			{
				OnItemSelected?.Invoke(typePopupItem);
			}
		}
	}
}