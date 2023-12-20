// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor.IMGUI.Controls;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	internal sealed class NullAdvancedDropdownItem : AdvancedDropdownItem
	{
		public const string DISPLAY_NAME = "<null>";

		public static void AddAsChild(AdvancedDropdownItem dropdown, int itemId) =>
			dropdown.AddChild(new AdvancedTypeDropdownItem(null, DISPLAY_NAME) { id = itemId });

		public NullAdvancedDropdownItem(string name) : base(name) { }
	}
}