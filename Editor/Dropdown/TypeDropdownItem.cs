// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.SerializeReference.Extensions.Editor.Menu;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	internal sealed class TypeDropdownItem : AdvancedDropdownItem
	{
		public readonly Type Type;

		public TypeDropdownItem(Type type, string name) : base(name)
		{
			Type = type;
			if (Type != null)
			{
				icon = (Texture2D) EditorIcons.SCRIPT_ICON.image;
			}
		}
	}
}