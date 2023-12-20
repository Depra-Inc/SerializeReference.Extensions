// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Inspector.SerializedReference
{
	/// <summary>
	/// An attribute that overrides the type name and category displayed in the <see cref="SubtypeDropdownAttribute"/> popup.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
		Inherited = false)]
	public sealed class SubtypeAliasAttribute : Attribute
	{
		public SubtypeAliasAttribute(string alias) => Alias = alias;

		public string Alias { get; }
	}
}