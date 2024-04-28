// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Depra.SerializeReference.Extensions
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeReferenceOrderAttribute : Attribute
	{
		private readonly int _order;

		public SerializeReferenceOrderAttribute(int order) => _order = order;

		public static IEnumerable<Type> OrderBy(IEnumerable<Type> self) => self.OrderBy(type =>
			type?.GetCustomAttribute<SerializeReferenceOrderAttribute>()?._order ?? 0);
	}
}