// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEditor.IMGUI.Controls;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	internal sealed class NullDropdownItem : AdvancedDropdownItem
	{
		public const string DISPLAY_NAME = "<null>";

		public NullDropdownItem() : base(DISPLAY_NAME) { }
	}
}