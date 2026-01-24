// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class TypeExtensions
	{
		public static object CreateInstance(this Type self)
		{
			object newObject;
			//if (self?.GetConstructor(Type.EmptyTypes) != null)
			{
				newObject = Activator.CreateInstance(self);
			}
			// else
			// {
			// 	newObject = self != null ? FormatterServices.GetUninitializedObject(self) : null;
			// }

			return newObject;
		}

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
			attribute = GetCustomAttribute<TAttribute>(self);
			return attribute != null;
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this Type self) where TAttribute : class =>
			Attribute.GetCustomAttribute(self, typeof(TAttribute)) as TAttribute;
		
		public static IEnumerable<Type> GetAllTypesInCurrentDomain()
		{
			var currentDomain = AppDomain.CurrentDomain;
			// if (cachedDomainTypes.TryGetValue(currentDomain, out var cachedTypes))
			// {
			// 	return cachedTypes;
			// }

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var types = new List<Type>();
			foreach (var assembly in assemblies)
			{
				try
				{
					types.AddRange(assembly.GetTypes());
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}

			//cachedDomainTypes.Add(currentDomain, types);

			return types;
		}
		
		private static IReadOnlyList<Type> _systemObjectTypes;

		public static IReadOnlyList<Type> GetAllSystemObjectTypes()
		{
			if (_systemObjectTypes == null)
			{
				var assemblies = CompilationPipeline.GetAssemblies();
				var playerAssemblies = assemblies.Where(t => !t.flags.HasFlag(AssemblyFlags.EditorAssembly))
					.Select(t => t.name).ToArray();
				var baseType = typeof(object);
				var typesCollection = TypeCache.GetTypesDerivedFrom(baseType);
				var customTypes = typesCollection.Where(IsValidTypeForGenericParameter).OrderBy(t => t.FullName);

				var typesList = new List<Type>();
				typesList.AddRange(GetBuiltInUnitySerializeTypes());
				typesList.AddRange(customTypes);
				_systemObjectTypes = typesList.ToArray();

				bool IsValidTypeForGenericParameter(Type t)
				{
					var isUnityObjectType = t.IsSubclassOf(typeof(UnityEngine.Object));
					var isFinalSerializeType = !t.IsAbstract && !t.IsInterface && !t.IsGenericType && t.IsSerializable;
					var isEnum = t.IsEnum;
					var isTargetType = playerAssemblies.Any(asm => t.Assembly.FullName.StartsWith(asm)) ||
					                   t.Assembly.FullName.StartsWith(nameof(UnityEngine));

					return isTargetType && (isFinalSerializeType || isEnum || isUnityObjectType);
				}
			}

			return _systemObjectTypes;
		}
		
		private static Type[] GetBuiltInUnitySerializeTypes()
		{
			return GetDefaultTypes();
		}

		private static Type[] GetDefaultTypes()
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