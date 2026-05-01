using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Depra.SerializeReference.Extensions.Editor.Internal;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.SerializeReference.Extensions.Editor.Dropdown
{
	internal sealed class GenericTypeCreateWindow : EditorWindow
	{
		private const int INVALID_INDEX = -1;

		private Type _inputGenericType;
		private SerializedProperty _property;
		private Action<Type> _onSelectNewGenericType;

		private IReadOnlyList<Type> _specifiedTypesFromProperty;

		private int[] _selectedIndexes;
		private List<IReadOnlyList<Type>> _typesForParameters;
		private List<IReadOnlyList<string>> _typeNamesForParameters;

		private Button _generateGenericTypeButton;
		private IReadOnlyList<Button> _parameterTypeButtons;
		private IReadOnlyList<Toggle> _makeArrayTypeToggles;

		public static void Open(SerializedProperty property, Rect propertyRect,
			Type genericType, Action<Type> onSelectedConcreteType)
		{
			var window = GetWindow<GenericTypeCreateWindow>();
			window.titleContent = new GUIContent("Create Generic Type");
			window.Show();
			window.Initialize(property, genericType, onSelectedConcreteType);
			window.CreateElements();

			var currentRect = window.position;
			var windowPos = GUIUtility.GUIToScreenPoint(propertyRect.position);
			currentRect.position = windowPos;
			window.position = currentRect;
		}

		private static string GetTypeName(Type type) => type.FullName;

		private void Initialize(SerializedProperty property, Type genericType, Action<Type> onSelectedConcreteType)
		{
			_property = property;
			_inputGenericType = genericType;
			_onSelectNewGenericType = onSelectedConcreteType;

			var genericArgs = _inputGenericType.GetGenericArguments();
			_selectedIndexes = new int[genericArgs.Length];

			FillTypesAndNames();
			FillSpecifiedTypesFromProperty();
		}

		private void FillTypesAndNames()
		{
			var genericParams = _inputGenericType.GetGenericArguments();
			_typesForParameters = new List<IReadOnlyList<Type>>();
			_typeNamesForParameters = new List<IReadOnlyList<string>>();

			for (var index = 0; index < _selectedIndexes.Length; index++)
			{
				_selectedIndexes[index] = INVALID_INDEX;
				IReadOnlyList<Type> targetTypes;
				var genericParam = genericParams[index];
				if (genericParam.GetInterfaces().Length == 0)
				{
					targetTypes = TypeUtils.GetAllSystemObjectTypes();
				}
				else
				{
					var systemObjectTypes = TypeUtils.GetAllSystemObjectTypes();
					targetTypes = TypeCache.GetTypesDerivedFrom(genericParam)
						.Where(t => systemObjectTypes.Contains(t))
						.ToArray();
				}

				_typesForParameters.Add(targetTypes);
				var names = targetTypes.Select(GetTypeName).ToArray();
				_typeNamesForParameters.Add(names);
			}
		}

		private void FillSpecifiedTypesFromProperty()
		{
			var propertyType = TypeUtils.ExtractTypeFromString(_property.managedReferenceFieldTypename);
			if (!propertyType.IsGenericType || !propertyType.IsInterface)
			{
				_specifiedTypesFromProperty = new Type[_selectedIndexes.Length];
				return;
			}

			var genericInterfaces = _inputGenericType.GetInterfaces();
			var genericInterfaceIndex = Array.FindIndex(genericInterfaces, IsSameGenericInterface);
			var genericInterface = genericInterfaces[genericInterfaceIndex];
			var genericInterfaceArgs = genericInterface.GetGenericArguments();

			var propertyGenericArgs = propertyType.GetGenericArguments();
			var genericTypeArgs = _inputGenericType.GetGenericArguments();
			var specifiedTypes = new Type[genericTypeArgs.Length];
			for (var i = 0; i < genericInterfaceArgs.Length; i++)
			{
				var genericArg = genericInterfaceArgs[i];
				var specifiedType = propertyGenericArgs[i];
				var genericArgIndex = Array.FindIndex(genericTypeArgs, Match);
				specifiedTypes[genericArgIndex] = specifiedType;

				bool Match(Type type)
				{
					return type.Name == genericArg.Name;
				}
			}

			_specifiedTypesFromProperty = specifiedTypes;

			bool IsSameGenericInterface(Type type)
			{
				return type.IsGenericType && propertyType.GetGenericTypeDefinition() == type.GetGenericTypeDefinition();
			}
		}


		private void OnGUI()
		{
			if (IsDisposedProperty())
			{
				Close();
			}

			bool IsDisposedProperty()
			{
				if (_property == null)
				{
					return true;
				}

				var propertyType = _property.GetType();
				var isValidField = propertyType.GetProperty("isValid", BindingFlags.NonPublic | BindingFlags.Instance);
				var isValidValue = isValidField?.GetValue(_property);
				return isValidValue != null && !(bool)isValidValue;
			}
		}


		private void CreateElements()
		{
			rootVisualElement.Clear();
			CreateParameterButtons();
			CreateGenerateGenericTypeButton();
		}

		private void CreateParameterButtons()
		{
			var parameterButtons = new List<Button>();
			var arrayToggles = new List<Toggle>();
			_parameterTypeButtons = parameterButtons;
			_makeArrayTypeToggles = arrayToggles;

			var genericParams = _inputGenericType.GetGenericArguments();
			for (var i = 0; i < genericParams.Length; i++)
			{
				var index = i;
				var currentParam = genericParams[i];
				var paramName = $"[{i}] {currentParam.Name}";
				var button = new Button();

				button.clickable.clicked += () => ShowTypesForParamIndex(index, button);

				var parameterTypeLabel = new TextElement { text = paramName };
				var makeArrayToggle = new Toggle("Make Array Type");

				var group = new Box();
				group.style.flexDirection = FlexDirection.Row;
				group.style.alignItems = Align.Center;
				group.Add(parameterTypeLabel);
				group.Add(button);
				group.Add(makeArrayToggle);

				parameterButtons.Add(button);
				arrayToggles.Add(makeArrayToggle);

				rootVisualElement.Add(group);

				RefreshGenericParameterButton(i);
			}
		}

		private void CreateGenerateGenericTypeButton()
		{
			_generateGenericTypeButton = new Button { text = "Generate" };
			_generateGenericTypeButton.clickable.clicked += GenerateGenericType;
			rootVisualElement.Add(_generateGenericTypeButton);

			RefreshGenerateGenericButton();
		}

		private void GenerateGenericType()
		{
			var parameterTypes = new Type[_selectedIndexes.Length];
			for (var index = 0; index < parameterTypes.Length; index++)
			{
				Type type;
				if (_specifiedTypesFromProperty[index] != null)
				{
					type = _specifiedTypesFromProperty[index];
				}
				else
				{
					var typeIndex = _selectedIndexes[index];
					type = _typesForParameters[index][typeIndex];
					if (!type.IsArray && _makeArrayTypeToggles[index].value)
					{
						type = type.MakeArrayType();
					}
				}

				parameterTypes[index] = type;
			}

			var newGenericType = _inputGenericType.MakeGenericType(parameterTypes);
			try
			{
				_onSelectNewGenericType.Invoke(newGenericType);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}

			Close();
		}

		private void ShowTypesForParamIndex(int genericParamIndex, Button selectedButton)
		{
			var currentTypeNames = _typeNamesForParameters[genericParamIndex];
			var dropdown = new AdvancedTypeDropdown(currentTypeNames, new AdvancedDropdownState(), ApplySelectedTypeIndex);
			dropdown.Show(new Rect(selectedButton.transform.position, selectedButton.transform.scale));

			void ApplySelectedTypeIndex(Type item)
			{
				//_selectedIndexes[genericParamIndex] = item.id;
				RefreshGenericParameterButton(genericParamIndex);
				RefreshGenerateGenericButton();
			}
		}

		private void RefreshGenericParameterButton(int parameterIndex)
		{
			var button = _parameterTypeButtons[parameterIndex];
			var specifiedType = _specifiedTypesFromProperty[parameterIndex];
			if (specifiedType != null)
			{
				button.text = GetTypeName(specifiedType);
				button.SetEnabled(false);
				_makeArrayTypeToggles[parameterIndex].SetEnabled(false);
				return;
			}

			var selectedIndex = _selectedIndexes[parameterIndex];
			var buttonText = selectedIndex == INVALID_INDEX
				? "Select Type"
				: _typeNamesForParameters[parameterIndex][selectedIndex];
			button.text = buttonText;
		}

		private void RefreshGenerateGenericButton()
		{
			var specifiedTypesCount = _specifiedTypesFromProperty.Count(t => t != null);
			var selectedIndexesCount = _selectedIndexes.Count(t => t != INVALID_INDEX);
			var isSelectedAllTypes = (specifiedTypesCount + selectedIndexesCount) == _selectedIndexes.Length;
			_generateGenericTypeButton.SetEnabled(isSelectedAllTypes);
		}
	}
}