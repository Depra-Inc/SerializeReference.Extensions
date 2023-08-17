// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using UnityEngine;

namespace Depra.SerializeReference.Selection.Runtime
{
	/// <summary>
	/// Attribute to specify the type of the field serialized by the <see cref="SerializeReference"/> attribute in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SubtypeMenuAttribute : PropertyAttribute { }
}