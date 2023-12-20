// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.Inspector.SerializedReference
{
	/// <summary>
	/// Attribute to specify the type of the field serialized by the <see cref="SerializeReference"/> attribute in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SubtypeDropdownAttribute : PropertyAttribute { }
}