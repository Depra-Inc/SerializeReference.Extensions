using System;

namespace Depra.SerializeReference.Selection.Runtime
{
	/// <summary>
	/// An attribute that overrides the type name and category displayed in the <see cref="SubclassSelectionAttribute"/> popup.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,Inherited = false)]
	public sealed class AddTypeMenuAttribute : Attribute
	{
		private static readonly char[] SEPARATORS = { '/' };

		public AddTypeMenuAttribute(string menuName, int order = 0)
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