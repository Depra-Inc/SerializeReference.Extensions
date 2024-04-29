// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class Module
	{
		public const string MENU_PATH = nameof(SerializeReference) + SLASH + nameof(Extensions) + SLASH;

		internal const string SLASH = "/";
		internal const string UNDERSCORE = "_";
		internal static readonly char[] SEPARATORS = { '.', '/' };
	}
}