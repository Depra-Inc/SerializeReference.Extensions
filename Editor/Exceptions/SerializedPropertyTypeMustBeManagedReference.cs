﻿// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.SerializedReference.Dropdown.Editor.Exceptions
{
	internal sealed class SerializedPropertyTypeMustBeManagedReference : ArgumentException
	{
		private const string MESSAGE = "The serialized property type must be SerializedPropertyType.ManagedReference.";

		public SerializedPropertyTypeMustBeManagedReference(string paramName) : base(MESSAGE, paramName) { }
	}
}