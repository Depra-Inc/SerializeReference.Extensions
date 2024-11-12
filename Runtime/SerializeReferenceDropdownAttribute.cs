// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Depra.SerializeReference.Extensions
{
	/// <summary>
	/// Attribute to specify the type of the field serialized by the <see cref="SerializeReferenceAttribute"/> attribute in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeReferenceDropdownAttribute : SerializeReferenceAttribute
	{
		private static readonly Type UNITY_OBJECT_TYPE = typeof(Object);

		public override IEnumerable<Type> GetTypes(Type referenceType) =>
			from extractedTypes in TypeCache.GetTypesDerivedFrom(referenceType)
			where extractedTypes.IsPublic || extractedTypes.IsNestedPublic
			where !extractedTypes.IsAbstract && !extractedTypes.IsGenericType
			where !UNITY_OBJECT_TYPE.IsAssignableFrom(extractedTypes)
			where !IsDefined(extractedTypes, HideSerializeReferenceAttribute.TYPE)
			select extractedTypes;
	}
}