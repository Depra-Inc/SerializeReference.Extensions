// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal static class Module
	{
		public const string MENU_PATH = nameof(SerializeReferenceAttribute) + "/" + nameof(Extensions) + "/";
		internal static readonly char[] SEPARATORS = { '.', '/' };
	}
}