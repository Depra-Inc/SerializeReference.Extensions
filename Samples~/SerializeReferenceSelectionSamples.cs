// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using Depra.SerializedReference.Dropdown.Runtime;
using UnityEngine;

namespace Depra.SerializedReference.Dropdown.Samples
{
    internal sealed class SerializeReferenceSelectionSamples : MonoBehaviour
    {
        [UnityEngine.SerializeReference] [SubtypeMenu]
        private ISampleCommand _command;

        [UnityEngine.SerializeReference] [SubtypeMenu]
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