using System;
using UnityEditor;
using UnityEngine;

namespace Depra.SerializeReference.Extensions.Editor.Internal
{
	[InitializeOnLoad]
	internal sealed class CopyPasteContextMenu
	{
		private static (string json, Type type) _lastObject;

		static CopyPasteContextMenu()
		{
			EditorApplication.contextualPropertyMenu += ShowSerializeReferenceCopyPasteContextMenu;
		}

		private static void ShowSerializeReferenceCopyPasteContextMenu(GenericMenu menu, SerializedProperty property)
		{
			if (property.propertyType != SerializedPropertyType.ManagedReference)
			{
				return;
			}

			var copyProperty = property.Copy();
			var copyContent = new GUIContent("Copy Serialize Reference");
			menu.AddItem(copyContent, false, _ => { CopySerializeReference(copyProperty); }, null);
			var pasteContent = new GUIContent("Paste Serialize Reference");
			menu.AddItem(pasteContent, false, _ => PasteSerializeReference(copyProperty), null);
			if (!property.IsArrayElement())
			{
				return;
			}

			var duplicateContent = new GUIContent("Duplicate Serialize Reference Array Element");
			menu.AddItem(duplicateContent, false, _ => DuplicateSerializeReferenceArrayElement(copyProperty), null);
		}

		private static void CopySerializeReference(SerializedProperty property)
		{
			var refValue = property.managedReferenceValue;
			_lastObject.json = JsonUtility.ToJson(refValue);
			_lastObject.type = refValue?.GetType();
		}

		private static void PasteSerializeReference(SerializedProperty property)
		{
			try
			{
				if (_lastObject.type != null)
				{
					var pasteObj = JsonUtility.FromJson(_lastObject.json, _lastObject.type);
					property.managedReferenceValue = pasteObj;
				}
				else
				{
					property.managedReferenceValue = null;
				}

				property.serializedObject.ApplyModifiedProperties();
				property.serializedObject.Update();
			}
			catch (Exception)
			{
				// ignored
			}
		}

		private static void DuplicateSerializeReferenceArrayElement(SerializedProperty property)
		{
			var sourceElement = property.managedReferenceValue;
			var arrayProperty = property.GetArrayPropertyFromArrayElement();
			var newElementIndex = arrayProperty.arraySize;
			arrayProperty.arraySize = newElementIndex + 1;

			if (sourceElement != null)
			{
				property.serializedObject.ApplyModifiedProperties();
				property.serializedObject.Update();

				var json = JsonUtility.ToJson(sourceElement);
				var newObj = JsonUtility.FromJson(json, sourceElement.GetType());
				var newElementProperty = arrayProperty.GetArrayElementAtIndex(newElementIndex);
				newElementProperty.managedReferenceValue = newObj;
			}

			property.serializedObject.ApplyModifiedProperties();
			property.serializedObject.Update();
		}
	}

	internal static class SerializedPropertyExtensions
	{
		private const string ARRAY_PROPERTY_SUBSTRING = ".Array.data[";

		public static bool IsArrayElement(this SerializedProperty self) =>
			self.propertyPath.Contains(ARRAY_PROPERTY_SUBSTRING);

		public static SerializedProperty GetArrayPropertyFromArrayElement(this SerializedProperty self)
		{
			var path = self.propertyPath;
			var startIndexPropertyPath = path.IndexOf(ARRAY_PROPERTY_SUBSTRING, StringComparison.Ordinal);
			var propertyPath = path.Remove(startIndexPropertyPath);

			return self.serializedObject.FindProperty(propertyPath);
		}
	}
}