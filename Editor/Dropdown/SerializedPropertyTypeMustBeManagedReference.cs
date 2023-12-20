// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEditor;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	internal sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
	{
		public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(
			$"The serialized property type must be {nameof(SerializedPropertyType.ManagedReference)}", paramName) { }
	}
}