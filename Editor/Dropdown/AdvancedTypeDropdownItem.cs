// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Inspector.SerializedReference.Editor.Utils;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	internal sealed class AdvancedTypeDropdownItem : AdvancedDropdownItem
	{
		public readonly Type Type;

		public AdvancedTypeDropdownItem(Type type, string name) : base(name)
		{
			Type = type;

			if (Type != null)
			{
				icon = (Texture2D) EditorIcons.ScriptIcon.image;
			}
		}
	}
}