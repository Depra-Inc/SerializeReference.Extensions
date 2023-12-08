// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializedReference.Dropdown.Runtime;
using UnityEditor;

namespace Depra.SerializedReference.Dropdown.Editor.Extensions
{
	internal static class TypeExtensions
	{
		private const int NULL_ORDER = -999;
		private static readonly Type UNITY_OBJECT_TYPE = typeof(UnityEngine.Object);

		public static IEnumerable<Type> OrderByType(this IEnumerable<Type> self) =>
			self.OrderBy(type => type == null
				? NULL_ORDER
				: GetTypeMenuAliasAttribute(type)?.Order ?? SubtypeMenuAliasAttribute.DEFAULT_ORDER);

		public static SubtypeMenuAliasAttribute GetTypeMenuAliasAttribute(this Type self) =>
			Attribute.GetCustomAttribute(self, typeof(SubtypeMenuAliasAttribute)) as SubtypeMenuAliasAttribute;

		public static IEnumerable<Type> GetDerivedTypes(this Type self) => TypeCache
			.GetTypesDerivedFrom(self)
			.Where(type =>
				(type.IsPublic || type.IsNestedPublic) &&
				type.IsAbstract == false &&
				type.IsGenericType == false &&
				UNITY_OBJECT_TYPE.IsAssignableFrom(type) == false &&
				Attribute.IsDefined(type, typeof(SerializableAttribute)));

		public static string[] SplitTypePath(this Type self)
		{
			var typeMenu = GetTypeMenuAliasAttribute(self);
			if (typeMenu != null)
			{
				return typeMenu.SplitMenuName();
			}

			var splitIndex = self.FullName!.LastIndexOf('.');
			var result = splitIndex >= 0
				? new[] { self.FullName[..splitIndex], self.FullName[(splitIndex + 1)..] }
				: new[] { self.Name };

			return result;
		}
	}
}