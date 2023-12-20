// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Inspector.SerializedReference.Editor.Dropdown
{
	internal static class SubtypeAliasExtensions
	{
		/// <summary>
		/// Returns the menu name split by <see cref="separators"/>.
		/// </summary>
		public static string[] SplitDropdownName(this string self, char[] separators) =>
			string.IsNullOrWhiteSpace(self) == false
				? self.Split(separators, StringSplitOptions.RemoveEmptyEntries)
				: Array.Empty<string>();
	}
}