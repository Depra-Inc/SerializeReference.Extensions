// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Inspector.SerializedReference
{
	/// <summary>
	/// An attribute that overrides the type name and category displayed in the <see cref="SubtypeMenuAttribute"/> popup.
	/// </summary>
	[AttributeUsage(VALID_TARGETS, Inherited = false)]
	public sealed class SubtypeMenuAliasAttribute : Attribute
	{
		internal const int DEFAULT_ORDER = 0;
		internal const string NULL_DISPLAY_NAME = "<null>";

		private const AttributeTargets VALID_TARGETS = AttributeTargets.Class | AttributeTargets.Struct |
		                                               AttributeTargets.Enum | AttributeTargets.Interface;

		private static readonly char[] SEPARATORS = { '/' };

		public SubtypeMenuAliasAttribute(string menuName, int order = DEFAULT_ORDER)
		{
			MenuName = menuName;
			Order = order;
		}

		public int Order { get; }

		private string MenuName { get; }

		/// <summary>
		/// Returns the menu name split by <see cref="SEPARATORS"/>.
		/// </summary>
		public string[] SplitMenuName() => string.IsNullOrWhiteSpace(MenuName) == false
			? MenuName.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries)
			: Array.Empty<string>();

		/// <summary>
		/// Returns the display name without the path.
		/// </summary>
		public string GetTypeNameWithoutPath()
		{
			var splitMenuName = SplitMenuName();
			return splitMenuName.Length != 0 ? splitMenuName[^1] : null;
		}
	}
}