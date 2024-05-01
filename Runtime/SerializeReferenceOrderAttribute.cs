// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializeReference.Extensions
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeReferenceOrderAttribute : Attribute
	{
		public readonly int Order;

		public SerializeReferenceOrderAttribute(int order) => Order = order;
	}
}