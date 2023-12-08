// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.Inspector.SerializedReference.Samples
{
	internal sealed class SerializeReferenceSelectionSamples : MonoBehaviour
	{
		[SubtypeMenu] [SerializeReference] private ISampleCommand _command;
		[SubtypeMenu] [SerializeReference] private ISampleCommand[] _commands;

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

	[Serializable]
	public sealed class ClassCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(ClassCommand)}");
	}

	[Serializable]
	[SubtypeMenuAlias(nameof(CommandWithCustomTypeMenu))]
	public sealed class CommandWithCustomTypeMenu : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(CommandWithCustomTypeMenu)}");
	}

	[Serializable]
	public readonly struct StructCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(StructCommand)}");
	}

	[Serializable]
	public sealed record RecordCommand : ISampleCommand
	{
		void ISampleCommand.Execute() => Debug.Log($"{nameof(ISampleCommand.Execute)} {nameof(RecordCommand)}");
	}
}