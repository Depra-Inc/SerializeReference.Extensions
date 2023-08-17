// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializeReference.Selection.Runtime;
using UnityEditor;

namespace Depra.SerializeReference.Selection.Editor.Extensions
{
	internal static class TypeExtensions
	{
		public const string NULL_DISPLAY_NAME = "<null>";
		private static readonly Type UNITY_OBJECT_TYPE = typeof(UnityEngine.Object);

		public static IEnumerable<Type> OrderByType(this IEnumerable<Type> self) =>
			self.OrderBy(type => type == null ? -999 : GetTypeMenuAliasAttribute(type)?.Order ?? 0);

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