// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor.IMGUI.Controls;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	internal sealed class NullAdvancedDropdownItem : AdvancedDropdownItem
	{
		public const string DISPLAY_NAME = "<null>";

		public NullAdvancedDropdownItem() : base(DISPLAY_NAME) { }
	}
}