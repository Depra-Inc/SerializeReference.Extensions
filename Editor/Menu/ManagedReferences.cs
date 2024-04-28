using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Depra.SerializeReference.Extensions.Editor.Menu.Module;

namespace Depra.SerializeReference.Extensions.Editor.Menu
{
	internal static class ManagedReferences
	{
		[MenuItem(MENU_PATH + nameof(FindMissingTypesOnScriptableObjects))]
		public static void FindMissingTypesOnScriptableObjects()
		{
			var report = new StringBuilder();
			var searchInFolders = new[] { "Assets" };
			var filter = $"t:{nameof(ScriptableObject)}";
			var guids = AssetDatabase.FindAssets(filter, searchInFolders);
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadMainAssetAtPath(path);
				if (obj == null)
				{
					continue;
				}

				if (SerializationUtility.HasManagedReferencesWithMissingTypes(obj))
				{
					var missingTypes = SerializationUtility.GetManagedReferencesWithMissingTypes(obj);
					report.AppendLine($"Found {missingTypes.Length} missing references on {obj.GetType().Name}s:");
					report = LogMissingTypes(missingTypes, report);
				}
				else
				{
					report.Append("No missing types to clear on ").Append(path).AppendLine();
				}
			}

			if (report.Length == 0)
			{
				report.Append("Missing types not found.");
			}

			Debug.Log(report.ToString());
		}

		[MenuItem(MENU_PATH + nameof(ClearMissingTypesOnScriptableObjects))]
		public static void ClearMissingTypesOnScriptableObjects()
		{
			var report = new StringBuilder();
			var searchInFolders = new[] { "Assets" };
			var filter = $"t:{nameof(ScriptableObject)}";
			var guids = AssetDatabase.FindAssets(filter, searchInFolders);
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadMainAssetAtPath(path);
				if (obj == null)
				{
					continue;
				}

				if (SerializationUtility.ClearAllManagedReferencesWithMissingTypes(obj))
				{
					report.Append("Cleared missing types from ").Append(path).AppendLine();
				}
				else
				{
					report.Append("No missing types to clear on ").Append(path).AppendLine();
				}
			}

			if (report.Length == 0)
			{
				report.Append("Missing types not found.");
			}

			Debug.Log(report.ToString());
		}

		private static StringBuilder LogMissingTypes(IEnumerable<ManagedReferenceMissingType> types, StringBuilder report)
		{
			foreach (var type in types)
			{
				report.Append($"ID: {type.referenceId}; Name {type.className}").AppendLine();
			}

			return report;
		}
	}
}