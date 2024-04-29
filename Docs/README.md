# SerializeReference.Extensions

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Table of Contents</summary>

- [Introduction](#introduction)
- [Features](#-features)
- [Installation](#-installation)
- [FAQ](#-faq)
- [Contribute](#-contribute)
- [Support](#support)
- [License](#license)

</details>

## Introduction

The `SerializeReference` attribute, added in **Unity 2019.3**,
makes it possible to serialize references to interfaces and abstract classes.

The `SerializeReferenceDropdown` attribute allows you to easily set
subclasses of those abstract classes in the **Editor** that
are serialized by `SerializeReference` attribute.

**Inspired by [this repo](https://github.com/mackysoft/Unity-SerializeReferenceExtensions).**

## 🦾 Features

- Easily set subclass by popup.
- Type finding through fuzzy searching.
- The `SerializeReferenceDropdown` attribute supports types that meet the following conditions:
    - ✅ Public
    - ✅ Not abstract
    - ✅ Not generic
    - ✅ Not a Unity object
    - ✅ Serializable attribute is applied.
- Override the type name, path and order by the `SerializeReferenceMenuPath` attribute.

## 📥 Installation

Download any version from [releases](https://github.com/Depra-Inc/SerializeReference.Extensions/releases).

#### Install via git URL

Alternatively, you can add this package by opening the **PackageManager** and entering

`https://github.com/Depra-Inc/SerializeReference.Extensions.git`

from the `Add package from git URL` option.

## 🔰 Usage

1. Implement an interface or an abstract class:

```cs
internal interface ISampleCommand
{
    void Execute();
}
```

2. Define a serialized reference or an array of them:

```cs
[SerializeReferenceDropdown] [SerializeReference] private ISampleCommand _command;
[SerializeReferenceDropdown] [SerializeReference] private ISampleCommand[] _commands;
```

3. Implement public class or struct od record. They can be nested.

```cs
[Serializable]
public sealed class ClassCommand : ISampleCommand
{
    void ISampleCommand.Execute() { }
}

[Serializable]
public readonly struct StructCommand : ISampleCommand
{
    void ISampleCommand.Execute() { }
}

[Serializable]
public sealed record RecordCommand : ISampleCommand
{
    void ISampleCommand.Execute() { }
}
```

4. Add `SerializeReferenceMenuPath` attribute to override the type name, path, and order:

```cs
[Serializable]
[SerializeReferenceMenuPath(nameof(CommandWithCustomTypeMenu))]
public sealed class CommandWithCustomTypeMenu : ISampleCommand
{
    void ISampleCommand.Execute() => { }
}
```

## ❓ FAQ

### If the type is renamed, the reference is lost.

It is a limitation of `SerializeReference` of Unity.

When serializing a `SerializeReference` reference, the type name, namespace, and assembly name are used, so if any of
these are changed, the reference cannot be resolved during deserialization.

To solve this problem, `UnityEngine.Scripting.APIUpdating.MovedFromAttribute` can be used.

Also, [this thread](https://forum.unity.com/threads/serializereference-data-loss-when-class-name-is-changed.736874/)
will be helpful.

#### References

- https://forum.unity.com/threads/serializereference-data-loss-when-class-name-is-changed.736874/
- https://issuetracker.unity3d.com/issues/serializereference-serialized-reference-data-lost-when-the-class-name-is-refactored

## 🤝 Contribute

I welcome feature requests and bug reports in [issues](https://github.com/Depra-Inc/SerializeReference.Extensions/issues)
and [pull requests](https://github.com/Depra-Inc/SerializeReference.Extensions/pulls).

## Support

I am an independent developer,
and most of the development on this project is done in my spare time.
If you're interested in collaboration or hiring me for a project,
please check out [my portfolio](https://github.com/Depra-Inc) and [reach out](mailto:n.melnikov@depra.org)!

## License

**[Apache-2.0](https://github.com/Depra-Inc/SerializeReference.Extensions/blob/main/LICENSE)**

Copyright (c) 2023-2024 Nikolay Melnikov
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)