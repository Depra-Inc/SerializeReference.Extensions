// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.SerializeReference.Selection.Runtime;
using UnityEngine;

namespace Depra.SerializeReference.Selection.Samples
{
	internal sealed class SerializeReferenceSelectionSamples : MonoBehaviour
	{
		[SubclassSelection]
		[UnityEngine.SerializeReference] private IInterface _interface;

		[SubclassSelection]
		[UnityEngine.SerializeReference] private AbstractClass _abstractClass;

		internal abstract class AbstractClass { }

		[Serializable]
		[AddTypeMenu(nameof(ClassImplementation))]
		public sealed class ClassImplementation : AbstractClass { }

		private interface IInterface { }

		[Serializable]
		[AddTypeMenu(nameof(ClassInterfaceImplementation))]
		public sealed class ClassInterfaceImplementation : IInterface { }

		[Serializable]
		[AddTypeMenu(nameof(RecordInterfaceImplementation))]
		public sealed record RecordInterfaceImplementation : IInterface { }
	}
}