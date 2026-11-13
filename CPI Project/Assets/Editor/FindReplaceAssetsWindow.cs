using System;
using System.Collections.Generic;
using System.IO;
using Disney.Kelowna.Common;
using UnityEditor;
using UnityEngine;

public class FindReplaceAssetsWindow : EditorWindow
{
	private static readonly HashSet<string> TextExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		".asmdef", ".asmref", ".asset", ".c", ".cginc", ".compute", ".config", ".csv", ".cs", ".glsl",
		".h", ".hlsl", ".html", ".ini", ".js", ".md", ".shader", ".sql", ".text",
		".txt", ".uss", ".uxml", ".xml", ".yaml", ".yml"
	};

	private string findText = "";
	private string replaceText = "";
	private string resultText = "";

	[MenuItem("Project/Editor/Find and Replace Text")]
	private static void Open()
	{
		GetWindow<FindReplaceAssetsWindow>("Find and Replace Text");
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("Find and replace text", EditorStyles.boldLabel);
		findText = EditorGUILayout.TextField("Find", findText);
		replaceText = EditorGUILayout.TextField("Replace with", replaceText);

		EditorGUILayout.HelpBox("Scans common text file types. Binary assets are skipped.", MessageType.Info);
		if (!string.IsNullOrEmpty(resultText))
			EditorGUILayout.LabelField(resultText);

		using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(findText)))
		{
			if (GUILayout.Button("Replace All"))
				ReplaceAll();
		}
	}

	private void ReplaceAll()
	{
		string projectPath = Directory.GetParent(Application.dataPath).FullName;
		string clientInfoAssetPath = Path.GetFullPath(Path.Combine(projectPath, ClientInfo.CLIENT_INFO_EDITOR_PATH.Replace('/', Path.DirectorySeparatorChar)));
		List<string> files = new List<string>(Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories));
		string projectSettingsPath = Path.Combine(projectPath, "ProjectSettings");
		if (Directory.Exists(projectSettingsPath))
			files.AddRange(Directory.GetFiles(projectSettingsPath, "*", SearchOption.AllDirectories));
		files.Sort(StringComparer.OrdinalIgnoreCase);
		int changedFiles = 0;
		int replacements = 0;
		List<string> changedAssetPaths = new List<string>();
		List<string> skippedFiles = new List<string>();
		string assetsDirectoryPath = Path.GetFullPath(Application.dataPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

		try
		{
			for (int i = 0; i < files.Count; i++)
			{
				string file = files[i];
				EditorUtility.DisplayProgressBar("Find and Replace", Path.GetFileName(file), files.Count == 0 ? 1f : (float)i / files.Count);
				if (!TextExtensions.Contains(Path.GetExtension(file)))
					continue;

				if (string.Equals(file, clientInfoAssetPath, StringComparison.OrdinalIgnoreCase))
				{
					ClientInfo clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>(ClientInfo.CLIENT_INFO_EDITOR_PATH);
					if (clientInfo == null)
					{
						skippedFiles.Add(file + ": Unity could not load the ClientInfo asset.");
						continue;
					}

					int clientInfoReplacements = ReplaceClientInfoStrings(clientInfo, findText, replaceText);
					if (clientInfoReplacements > 0)
					{
						changedFiles++;
						replacements += clientInfoReplacements;
						changedAssetPaths.Add(ClientInfo.CLIENT_INFO_EDITOR_PATH);
					}
					continue;
				}

				string original;
				try
				{
					original = File.ReadAllText(file);
				}
				catch (IOException exception)
				{
					skippedFiles.Add(file + ": " + exception.Message);
					continue;
				}
				catch (UnauthorizedAccessException exception)
				{
					skippedFiles.Add(file + ": " + exception.Message);
					continue;
				}

				int count = CountOccurrences(original, findText);
				if (count == 0)
					continue;

				File.WriteAllText(file, original.Replace(findText, replaceText));
				changedFiles++;
				replacements += count;
				if (file.StartsWith(assetsDirectoryPath, StringComparison.OrdinalIgnoreCase))
				{
					string relativePath = file.Substring(assetsDirectoryPath.Length).Replace('\\', '/');
					changedAssetPaths.Add("Assets/" + relativePath);
				}
			}
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}

		for (int i = 0; i < changedAssetPaths.Count; i++)
			AssetDatabase.ImportAsset(changedAssetPaths[i], ImportAssetOptions.ForceUpdate);
		AssetDatabase.Refresh();
		resultText = "Replaced " + replacements + " occurrence(s) in " + changedFiles + " file(s).";
		if (skippedFiles.Count > 0)
		{
			resultText += " Could not process " + skippedFiles.Count + " file(s); see the Console.";
			Debug.LogWarning("Find and Replace could not process:\n" + string.Join("\n", skippedFiles.ToArray()));
		}
	}

	private static int ReplaceClientInfoStrings(ClientInfo clientInfo, string findText, string replaceText)
	{
		SerializedObject serializedObject = new SerializedObject(clientInfo);
		int replacements = 0;
		string[] stringPropertyNames =
		{
			"ClientVersion", "BuildVersion", "Changelist", "Platform", "BuildTargetGroup", "BuildPhase"
		};

		for (int i = 0; i < stringPropertyNames.Length; i++)
		{
			SerializedProperty property = serializedObject.FindProperty(stringPropertyNames[i]);
			if (property != null && property.propertyType == SerializedPropertyType.String)
				replacements += ReplaceSerializedString(property, findText, replaceText);
		}

		SerializedProperty gameAssemblyNames = serializedObject.FindProperty("GameAssemblyNames");
		if (gameAssemblyNames != null && gameAssemblyNames.isArray)
		{
			for (int i = 0; i < gameAssemblyNames.arraySize; i++)
			{
				SerializedProperty assemblyName = gameAssemblyNames.GetArrayElementAtIndex(i);
				if (assemblyName.propertyType == SerializedPropertyType.String)
					replacements += ReplaceSerializedString(assemblyName, findText, replaceText);
			}
		}

		if (replacements > 0)
		{
			serializedObject.ApplyModifiedProperties();
			AssetDatabase.SaveAssetIfDirty(clientInfo);
		}
		return replacements;
	}

	private static int ReplaceSerializedString(SerializedProperty property, string findText, string replaceText)
	{
		int count = CountOccurrences(property.stringValue, findText);
		if (count > 0)
			property.stringValue = property.stringValue.Replace(findText, replaceText);
		return count;
	}

	private static int CountOccurrences(string text, string value)
	{
		int count = 0;
		int index = 0;
		while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
		{
			count++;
			index += value.Length;
		}
		return count;
	}
}
