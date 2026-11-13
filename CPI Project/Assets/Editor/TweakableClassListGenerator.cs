using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweaker.Core;
using UnityEditor;

public static class TweakableClassListGenerator
{
	private const string ClassListAssetPath = "Assets/Generated/Resources/TweakableClassList.asset";

	[MenuItem("Project/Tools/Tweaker/Rebuild Tweakable Classes")]
	private static void ScanAndUpdateClassList()
	{
		TweakablesClassList classList = AssetDatabase.LoadAssetAtPath<TweakablesClassList>(ClassListAssetPath);
		if (classList == null)
		{
			return;
		}

		string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript");
		HashSet<string> discoveredClasses = new HashSet<string>();
		bool cancelled = false;
		try
		{
			for (int i = 0; i < scriptGuids.Length; i++)
			{
				string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuids[i]);
				float progress = scriptGuids.Length == 0 ? 1f : (float)i / scriptGuids.Length;
				if (EditorUtility.DisplayCancelableProgressBar("Scanning Tweakable Classes", scriptPath, progress))
				{
					cancelled = true;
					break;
				}

				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
				Type type = script == null ? null : script.GetClass();
				if (type != null && HasPublicTweakableMember(type) && !string.IsNullOrEmpty(type.AssemblyQualifiedName))
				{
					discoveredClasses.Add(type.AssemblyQualifiedName);
				}
			}
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}

		if (cancelled)
		{
			return;
		}

		List<string> rebuiltClasses = new List<string>();
		HashSet<string> allClasses = new HashSet<string>();
		if (classList.AssemblyList != null)
		{
			foreach (string existingClass in classList.AssemblyList)
			{
				if (!string.IsNullOrEmpty(existingClass) && allClasses.Add(existingClass))
				{
					rebuiltClasses.Add(existingClass);
				}
			}
		}

		foreach (string discoveredClass in discoveredClasses.OrderBy(className => className, StringComparer.Ordinal))
		{
			if (allClasses.Add(discoveredClass))
			{
				rebuiltClasses.Add(discoveredClass);
			}
		}

		if (classList.AssemblyList != null && classList.AssemblyList.SequenceEqual(rebuiltClasses))
		{
			return;
		}

		Undo.RecordObject(classList, "Rebuild Tweakable Class List");
		classList.AssemblyList = rebuiltClasses;
		EditorUtility.SetDirty(classList);
		AssetDatabase.SaveAssets();
	}

	private static bool HasPublicTweakableMember(Type type)
	{
		const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		MemberInfo[] members = type.GetMembers(flags);
		foreach (MemberInfo member in members)
		{
			bool isPublicTweakable =
				(member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property) &&
				member.GetCustomAttribute<TweakableAttribute>(false) != null &&
				member.GetCustomAttribute<PublicTweak>(false) != null;
			bool isPublicInvokable =
				member.MemberType == MemberTypes.Method &&
				member.GetCustomAttribute<InvokableAttribute>(false) != null &&
				member.GetCustomAttribute<PublicTweak>(false) != null;

			if (isPublicTweakable || isPublicInvokable)
			{
				return true;
			}
		}

		return false;
	}
}
