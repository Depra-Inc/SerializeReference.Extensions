// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	internal sealed class NullDropdownItem : AdvancedDropdownItem
	{
		public static readonly GUIContent CONTENT = new(DISPLAY_NAME, EditorIcons.NULL_ICON.image);
		private const string DISPLAY_NAME = "<null>";

		public NullDropdownItem() : base(DISPLAY_NAME) { }
	}
}