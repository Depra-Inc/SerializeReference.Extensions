// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Samples
{
	internal sealed class SerializeReferenceSelectionSamples : MonoBehaviour
	{
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ISampleCommand _command;

		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private ISampleCommand[] _commands;

		private void Start()
		{
			_command?.Execute();
			foreach (var command in _commands)
			{
				command?.Execute();
			}
		}

		[Serializable]
		public sealed class NestedCommand : ISampleCommand
		{
			void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(NestedCommand)}");
		}
	}

	internal interface ISampleCommand
	{
		void Execute();
	}

	public sealed class ClassCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(ClassCommand)}");
	}

	[SerializeReferenceMenuPath(nameof(Samples))]
	public sealed class CommandWithCustomTypeMenu : ISampleCommand
	{
		void ISampleCommand.Execute() =>
			Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(CommandWithCustomTypeMenu)}");
	}

	public readonly struct StructCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(StructCommand)}");
	}

	public sealed record RecordCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(RecordCommand)}");
	}
}