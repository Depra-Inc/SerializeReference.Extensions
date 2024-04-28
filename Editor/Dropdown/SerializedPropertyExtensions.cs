// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEditor;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	internal static class SerializedPropertyExtensions
	{
		private const string ARRAY_PROPERTY_SUBSTRING = ".Array.data[";

		public static bool IsArrayElement(this SerializedProperty self) =>
			self.propertyPath.Contains(ARRAY_PROPERTY_SUBSTRING);

		public static SerializedProperty GetArrayPropertyFromArrayElement(this SerializedProperty self)
		{
			var path = self.propertyPath;
			var startIndexPropertyPath = path.IndexOf(ARRAY_PROPERTY_SUBSTRING, StringComparison.Ordinal);
			var propertyPath = path.Remove(startIndexPropertyPath);

			return self.serializedObject.FindProperty(propertyPath);
		}
	}
}