// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Depra.SerializeReference.Extensions
{
	public abstract class SerializeReferenceAttribute : PropertyAttribute
	{
		public abstract IEnumerable<Type> GetTypes(Type referenceType);
	}
}