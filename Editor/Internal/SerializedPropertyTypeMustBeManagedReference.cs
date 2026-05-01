using System;
using UnityEditor;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	internal sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
	{
		public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(
			$"The serialized property type must be {nameof(SerializedPropertyType.ManagedReference)}",
			paramName) { }
	}
}