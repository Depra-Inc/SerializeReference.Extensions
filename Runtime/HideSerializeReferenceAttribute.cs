// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializeReference.Extensions
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
	public sealed class HideSerializeReferenceAttribute : Attribute
	{
		public static readonly Type TYPE = typeof(HideSerializeReferenceAttribute);
	}
}