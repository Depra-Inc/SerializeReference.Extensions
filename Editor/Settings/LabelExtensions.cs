// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.SerializeReference.Extensions.Editor.Settings
{
	internal static class LabelExtensions
	{
		public static VisualElement SetPropertiesStyle(this VisualElement self)
		{
			self.style.marginTop = 9;
			self.style.marginLeft = 9;

			return self;
		}

		public static Label SetHeaderStyle(this Label self)
		{
			self.style.fontSize = 19;
			self.style.marginTop = 1;
			self.style.marginLeft = 9;
			self.style.unityFontStyleAndWeight = FontStyle.Bold;

			return self;
		}
	}
}