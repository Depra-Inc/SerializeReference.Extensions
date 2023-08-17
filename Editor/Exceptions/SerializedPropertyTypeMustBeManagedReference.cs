// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;

namespace Depra.SerializeReference.Selection.Editor.Exceptions
{
	internal sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
	{
		private const string MESSAGE = "The serialized property type must be SerializedPropertyType.ManagedReference.";

		public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(MESSAGE, paramName) { }
	}
}