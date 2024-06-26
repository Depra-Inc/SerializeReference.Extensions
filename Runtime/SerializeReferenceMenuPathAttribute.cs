﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializeReference.Extensions
{
	/// <summary>
	/// An attribute that overrides the type name and category displayed in the <see cref="SerializeReferenceDropdownAttribute"/> popup.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	public sealed class SerializeReferenceMenuPathAttribute : Attribute
	{
		public readonly string Path;

		public SerializeReferenceMenuPathAttribute(string path) => Path = path;
	}
}