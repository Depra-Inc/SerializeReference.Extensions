// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class TypeExtensions
	{
		public static Type ExtractTypeFromString(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}

			var splitFieldTypename = typeName.Split(' ');
			var assemblyName = splitFieldTypename[0];
			assemblyName = assemblyName == "Assembly" ? "Assembly-CSharp" : assemblyName;

			var subStringTypeName = splitFieldTypename[1];
			if (splitFieldTypename.Length > 2)
			{
				subStringTypeName = typeName[(assemblyName.Length + 1)..];
			}

			var assembly = Assembly.Load(assemblyName);
			var targetType = assembly.GetType(subStringTypeName);

			return targetType;
		}

		public static object CreateInstance(this Type self) => Activator.CreateInstance(self);

		public static Type GetConcreteGenericType(Type propertyType, Type genericType)
		{
			if (propertyType.IsGenericType && CanCreateDirectGenericType())
			{
				return genericType.MakeGenericType(propertyType.GetGenericArguments());
			}

			return null;

			bool CanCreateDirectGenericType()
			{
				var genericArguments = genericType.GetInterfaces();
				var interfaceIndex = Array.FindIndex(genericArguments,
					argType => argType.IsGenericType &&
					           argType.GetGenericTypeDefinition() == propertyType.GetGenericTypeDefinition());
				var isHaveSameArgumentsCount =
					propertyType.GetGenericArguments().Length == genericType.GetGenericArguments().Length &&
					interfaceIndex != -1;
				var anyAbstract = propertyType.GetGenericArguments().Any(t => t.IsAbstract);

				return isHaveSameArgumentsCount && !anyAbstract;
			}
		}

		public static bool TryGetCustomAttribute<TAttribute>(this Type self, out TAttribute attribute)
			where TAttribute : class
		{
			attribute = self.GetCustomAttribute<TAttribute>();
			return attribute != null;
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this Type self) where TAttribute : class =>
			Attribute.GetCustomAttribute(self, typeof(TAttribute)) as TAttribute;

		private static IReadOnlyList<Type> _systemObjectTypes;

		public static IReadOnlyList<Type> GetAllSystemObjectTypes()
		{
			if (_systemObjectTypes != null)
			{
				return _systemObjectTypes;
			}

			var playerAssemblies = CompilationPipeline.GetAssemblies()
				.Where(a => !a.flags.HasFlag(AssemblyFlags.EditorAssembly))
				.Select(a => a.name)
				.ToHashSet();

			var result = new List<Type>(512);
			result.AddRange(GetBuiltInUnitySerializeTypes());
			result.AddRange(TypeCache.GetTypesDerivedFrom<object>().Where(type => IsValid(type, playerAssemblies)));
			result.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));

			return _systemObjectTypes = result;

			static bool IsValid(Type t, HashSet<string> assemblies)
			{
				var asmName = t.Assembly.GetName().Name;
				if (!assemblies.Contains(asmName) && asmName != nameof(UnityEngine))
				{
					return false;
				}

				if (t.IsEnum || typeof(UnityEngine.Object).IsAssignableFrom(t))
				{
					return true;
				}

				return t.IsSerializable &&
				       !t.IsAbstract &&
				       !t.IsInterface &&
				       !t.IsGenericType;
			}
		}

		private static Type[] GetBuiltInUnitySerializeTypes()
		{
			return new[]
			{
				typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
				typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(string),

				typeof(Color), typeof(Color32), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Quaternion),
				typeof(Ray), typeof(Ray2D)
			};
		}
	}
}