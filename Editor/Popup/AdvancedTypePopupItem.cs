// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.SerializedReference.Dropdown.Editor.Utils;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializedReference.Dropdown.Editor.Popup
{
	internal sealed class AdvancedTypePopupItem : AdvancedDropdownItem
	{
		public readonly Type Type;

		public AdvancedTypePopupItem(Type type, string name) : base(name)
		{
			Type = type;

			if (Type != null)
			{
				icon = (Texture2D) EditorIcons.ScriptIcon.image;
			}
		}
	}
}