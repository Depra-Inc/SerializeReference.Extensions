// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using UnityEditor.IMGUI.Controls;

namespace Depra.SerializedReference.Dropdown.Editor.Popup
{
	internal sealed class AdvancedTypePopupItem : AdvancedDropdownItem
	{
		public readonly Type Type;

		public AdvancedTypePopupItem(Type type, string name) : base(name) =>
			Type = type;
	}
}