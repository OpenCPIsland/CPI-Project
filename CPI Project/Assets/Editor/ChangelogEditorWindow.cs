using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ChangelogEditorWindow : EditorWindow
{
    [Serializable]
    private class Entry
    {
        public string date = DateTime.Today.ToString("yyyy-MM-dd");
        public string notes = string.Empty;
    }

    [Serializable]
    private class EntryCollection
    {
        public List<Entry> entries = new List<Entry>();
    }

    private const string OutputPath = "Assets/Generated/Resources/Changelog/Changelog.json";
    private EntryCollection data = new EntryCollection();
    private Vector2 scroll;
    private Dictionary<int, Vector2> notesScrollPositions = new Dictionary<int, Vector2>();

    [MenuItem("Project/Island Changes Editor")]
    private static void Open()
    {
        GetWindow<ChangelogEditorWindow>("Island Changes Editor");
    }

    private void OnEnable()
    {
        LoadFromJson();
    }

    private void LoadFromJson()
    {
        if (File.Exists(OutputPath))
        {
            try
            {
                string json = File.ReadAllText(OutputPath);
                int braceIndex = json.IndexOf('{');
                if (braceIndex >= 0)
                {
                    data.entries.Clear();
                    int position = braceIndex + 1;
                    while (position < json.Length)
                    {
                        SkipWhitespace(json, ref position);
                        if (position >= json.Length || json[position] == '}')
                            break;

                        string date = ReadString(json, ref position);
                        SkipWhitespace(json, ref position);
                        if (position >= json.Length || json[position++] != ':')
                            break;
                        SkipWhitespace(json, ref position);
                        string notes = ReadString(json, ref position);
                        SkipWhitespace(json, ref position);
                        if (position < json.Length && json[position] == ',')
                            position++;

                        data.entries.Add(new Entry
                        {
                            date = date,
                            notes = notes
                        });
                    }
                    
                    data.entries.Sort((a, b) => CompareDates(b.date, a.date));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load changelog: " + e.Message);
            }
        }
    }

    private static int CompareDates(string dateA, string dateB)
    {
        DateTime dtA = ParseDate(dateA);
        DateTime dtB = ParseDate(dateB);
        return dtA.CompareTo(dtB);
    }

    private static void SkipWhitespace(string json, ref int position)
    {
        while (position < json.Length && char.IsWhiteSpace(json[position]))
            position++;
    }

    private static string ReadString(string json, ref int position)
    {
        if (position >= json.Length || json[position] != '"')
            return string.Empty;

        int start = position++;
        bool escaped = false;
        while (position < json.Length)
        {
            char current = json[position++];
            if (current == '"' && !escaped)
                break;
            if (current == '\\' && !escaped)
                escaped = true;
            else
                escaped = false;
        }

        StringValue decoded = JsonUtility.FromJson<StringValue>("{\"value\":" + json.Substring(start, position - start) + "}");
        return decoded != null ? decoded.value : string.Empty;
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < data.entries.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Entry " + (i + 1), EditorStyles.boldLabel);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                data.entries.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            data.entries[i].date = EditorGUILayout.TextField("Date", data.entries[i].date);
            if (!notesScrollPositions.ContainsKey(i))
                notesScrollPositions[i] = Vector2.zero;
            notesScrollPositions[i] = EditorGUILayout.BeginScrollView(notesScrollPositions[i], GUILayout.Height(200));
            data.entries[i].notes = EditorGUILayout.TextArea(data.entries[i].notes, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Entry"))
        {
            data.entries.Insert(0, new Entry());
        }

        if (GUILayout.Button("Reload from JSON"))
        {
            LoadFromJson();
        }

        if (GUILayout.Button("Generate"))
            Generate();
    }

    private void Generate()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Generating Changelog", "Preparing entries", 0f);
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.AppendLine("{");
            for (int i = data.entries.Count - 1; i >= 0; i--)
            {
                EditorUtility.DisplayProgressBar("Generating Changelog", "Processing entry " + (data.entries.Count - i), data.entries.Count == 0 ? 1f : (float)(data.entries.Count - i) / data.entries.Count);
                Entry entry = data.entries[i];
                string date = ParseDate(entry.date).ToString("MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                output.Append("  ").Append(SerializeString(date));
                output.Append(": ").Append(SerializeString(entry.notes ?? string.Empty));
                if (i > 0)
                    output.Append(',');
                output.AppendLine();
            }
            output.AppendLine("}");

            string directory = Path.GetDirectoryName(OutputPath);
            Directory.CreateDirectory(directory);
            File.WriteAllText(OutputPath, output.ToString());
            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static DateTime ParseDate(string value)
    {
        DateTime parsed;
        return DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed) ? parsed : DateTime.Today;
    }

    private static string SerializeString(string value)
    {
        string wrapped = JsonUtility.ToJson(new StringValue { value = value });
        return wrapped.Substring(wrapped.IndexOf(':') + 1).TrimEnd('}');
    }

    [Serializable]
    private class StringValue
    {
        public string value;
    }
}
