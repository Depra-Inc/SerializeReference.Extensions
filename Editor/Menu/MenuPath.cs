// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializeReference.Extensions.Editor.Menu
{
	internal static class MenuPath
	{
		/// <summary>
		/// Returns the menu name split by <see cref="separators"/>.
		/// </summary>
		public static string[] SplitName(string name, char[] separators) => string.IsNullOrWhiteSpace(name) == false
			? name.Split(separators, StringSplitOptions.RemoveEmptyEntries)
			: Array.Empty<string>();
	}
}