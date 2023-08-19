# Depra.SerializedReference.Dropdown

<div align="center">
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Содержание</summary>

- [Введение](#введение)
- [Особенности](#-особенности)
- [Установка](#-установка)
- [Часто задаваемые вопросы](#-faq)
- [Сотрудничество](#-сотрудничество)
- [Поддержка](#поддержка)
- [Лицензия](#лицензия)

</details>

## Введение

Атрибут `SerializeReference`, добавленный в **Unity 2019.3**,
позволяет сериализовать ссылки на интерфейсы и абстрактные классы.

Атрибут `SubtypeMenu` позволяет легко задавать подклассы
этих абстрактных классов в **Редакторе**, которые
сериализуются атрибутом `SerializeReference`.

**Вдохновлено [этим репозиторием](https://github.com/mackysoft/Unity-SerializeReferenceExtensions).**

## 🦾 Особенности

- Легко задавать подкласс с помощью выпадающего списка.
- Поиск типа с помощью нечеткого поиска.
- Атрибут `SubtypeMenu` поддерживает типы, которые соответствуют следующим условиям:
    - ✅ Публичные
    - ✅ Не абстрактные
    - ✅ Не обобщенные
    - ✅ Не являются объектами Unity
    - ✅ Применен атрибут Serializable.
- Переопределение имени, пути и порядка с помощью атрибута `SubtypeMenuAlias`.

## 📥 Установка

Скачайте любую версию из [релизов](https://github.com/Depression-aggression/SerializedReference.Dropdown/releases).

#### Установка через URL git

Также вы можете добавить этот пакет, открыв **PackageManager** и введя

`https://github.com/Depression-aggression/SerializedReference.Dropdown.git`

в опцию `Добавить пакет из URL git`.

## 🔰 Использование

1. Реализуйте интерфейс или абстрактный класс:

```cs
internal interface ISampleCommand
{
    void Execute();
}
```

2. Определите сериализованную ссылку или массив из них:

```cs
[SubtypeMenu] [SerializeReference] private ISampleCommand _command;
[SubtypeMenu] [SerializeReference] private ISampleCommand[] _commands;
```

3. Реализуйте публичный класс, структуру или запись. Они могут быть вложенными.

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

4. Добавьте атрибут SubtypeMenuAlias, чтобы переопределить имя типа, путь и порядок:

```cs
[Serializable]
[SubtypeMenuAlias(nameof(CommandWithCustomTypeMenu))]
public sealed class CommandWithCustomTypeMenu : ISampleCommand
{
    void ISampleCommand.Execute() => { }
}
```

## ❓ FAQ

### При переименовании типа теряется ссылка.

Это ограничение `SerializeReference` в Unity.

При сериализации ссылки `SerializeReference` используется имя типа, пространство имен и имя сборки, поэтому если
какое-либо из них изменяется, ссылку невозможно разрешить во время десериализации.

Для решения этой проблемы можно использовать `UnityEngine.Scripting.APIUpdating.MovedFromAttribute`.

Также
полезна [эта ветка](https://forum.unity.com/threads/serializereference-data-loss-when-class-name-is-changed.736874/).

#### Ссылки

- https://forum.unity.com/threads/serializereference-data-loss-when-class-name-is-changed.736874/
- https://issuetracker.unity3d.com/issues/serializereference-serialized-reference-data-lost-when-the-class-name-is-refactored

## 🤝 Сотрудничество

Я рад приветствовать запросы на добавление новых функций и сообщения об ошибках в
разделе [issues](https://github.com/Depression-aggression/SerializedReference.Dropdown/issues) и также
принимать [pull requests](https://github.com/Depression-aggression/SerializedReference.Dropdown/pulls).

## Поддержка

Я являюсь независимым разработчиком,
и большая часть разработки этого проекта выполняется в моём свободном времени.
Если вас интересует сотрудничество или найм меня для выполнения проекта,
пожалуйста, ознакомьтесь с [моим портфолио](https://github.com/Depression-aggression)
и [свяжитесь со мной](mailto:g0dzZz1lla@yandex.ru)!

## Лицензия

**[Apache-2.0](https://github.com/Depression-aggression/SerializedReference.Dropdown/blob/main/LICENSE)**

Авторские права (c) 2023 Николай Мельников
[g0dzZz1lla@yandex.ru](mailto:g0dzZz1lla@yandex.ru)