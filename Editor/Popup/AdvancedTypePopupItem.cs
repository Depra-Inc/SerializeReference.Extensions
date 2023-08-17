using System;
using UnityEditor.IMGUI.Controls;

namespace Depra.SerializeReference.Selection.Editor.Popup
{
	internal sealed class AdvancedTypePopupItem : AdvancedDropdownItem
	{
		public readonly Type Type;

		public AdvancedTypePopupItem(Type type, string name) : base(name) =>
			Type = type;
	}
}