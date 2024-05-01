// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class TypeExtensions
	{
		public static bool TryGetCustomAttribute<TAttribute>(this Type self, out TAttribute attribute)
			where TAttribute : class
		{
			attribute = GetCustomAttribute<TAttribute>(self);
			return attribute != null;
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this Type self) where TAttribute : class =>
			Attribute.GetCustomAttribute(self, typeof(TAttribute)) as TAttribute;
	}
}