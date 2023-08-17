using System;

namespace Depra.SerializeReference.Selection.Editor.Exceptions
{
	internal sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
	{
		private const string MESSAGE = "The serialized property type must be SerializedPropertyType.ManagedReference.";

		public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(MESSAGE, paramName) { }
	}
}