// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

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