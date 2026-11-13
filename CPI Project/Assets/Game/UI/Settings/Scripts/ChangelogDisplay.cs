using ClubPenguin.Accessibility;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangelogDisplay : MonoBehaviour
{
    public static string GetLatestEntryId()
    {
        TextAsset json = Resources.Load<TextAsset>("Changelog/Changelog");
        if (json == null)
            return string.Empty;

        List<KeyValuePair<string, string>> entries = ParseEntries(json.text);
        return entries.Count > 0 ? entries[entries.Count - 1].Key : string.Empty;
    }

    private sealed class ScaledText
    {
        public TMP_Text Text;
        public float BaseFontSize;
    }

    private sealed class ScaledEntry
    {
        public LayoutElement Layout;
        public TMP_Text Notes;
        public RectTransform LinesContainer;
        public List<ScaledLine> Lines;
    }

    private sealed class ScaledLine
    {
        public RectTransform Row;
        public LayoutElement Layout;
        public LayoutElement SymbolLayout;
        public TMP_Text Symbol;
        public TMP_Text Notes;
    }

    [Serializable]
    private class StringValue
    {
        public string value;
    }

    public GameObject entryGameObject;
    public TMP_Text dateText;
    public TMP_Text notesText;

    [Header("Notes Formatting")]
    public TMP_Text lineSymbolText;

    [Header("Entry Colors")]
    public GameObject colorObjectOne;
    public GameObject colorObjectTwo;
    public Color colorOne = new Color32(0x71, 0xC3, 0xFF, 0xFF);
    public Color colorTwo = new Color32(0x9C, 0xD5, 0xFF, 0xFF);

    private readonly List<ScaledText> scaledTexts = new List<ScaledText>();
    private readonly List<ScaledEntry> scaledEntries = new List<ScaledEntry>();
    private EventDispatcher eventDispatcher;
    private float accessibilityScale = 1f;

    private void Start()
    {
        accessibilityScale = PlayerPrefs.GetFloat(
            GetPlatformKey(AccessibilityConstants.SCALE_PLAYERPREFS_KEY),
            AccessibilityConstants.SCALE_MEDIUM);
        eventDispatcher = Service.Get<EventDispatcher>();
        eventDispatcher.AddListener<AccessibilityEvents.AccessibilityScaleUpdated>(onAccessibilityScaleUpdated);

        TMP_Text[] screenTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in screenTexts)
        {
            if (entryGameObject == null || !text.transform.IsChildOf(entryGameObject.transform))
                RegisterText(text);
        }

        if (lineSymbolText != null)
            lineSymbolText.gameObject.SetActive(false);

        TextAsset json = Resources.Load<TextAsset>("Changelog/Changelog");
        if (json == null || entryGameObject == null || dateText == null || notesText == null)
            return;

        List<KeyValuePair<string, string>> entries = ParseEntries(json.text);
        entries.Reverse();
        Transform parent = entryGameObject.transform.parent;
        entryGameObject.SetActive(false);

        string datePath = GetRelativePath(entryGameObject.transform, dateText.transform);
        string notesPath = GetRelativePath(entryGameObject.transform, notesText.transform);
        string colorObjectOnePath = GetRelativePath(entryGameObject.transform, colorObjectOne != null ? colorObjectOne.transform : null);
        string colorObjectTwoPath = GetRelativePath(entryGameObject.transform, colorObjectTwo != null ? colorObjectTwo.transform : null);
        bool lastEntryUsedColorOne = true;

        for (int i = 0; i < entries.Count; i++)
        {
            GameObject instance = Instantiate(entryGameObject, parent);
            TMP_Text instanceDate = FindText(instance.transform, datePath);
            TMP_Text instanceNotes = FindText(instance.transform, notesPath);
            bool useColorOne = !lastEntryUsedColorOne;
            Color rowColor = useColorOne ? colorOne : colorTwo;
            ApplyImageColor(instance.transform, colorObjectOnePath, rowColor);
            ApplyImageColor(instance.transform, colorObjectTwoPath, rowColor);
            lastEntryUsedColorOne = useColorOne;
            
            if (instanceDate != null)
            {
                instanceDate.text = entries[i].Key;
                RegisterText(instanceDate);
            }

            instance.SetActive(true);
            LayoutElement layout = instance.GetComponent<LayoutElement>();
            if (instanceNotes != null)
            {
                if (lineSymbolText != null)
                    CreateNoteLines(instance.transform, instanceNotes, entries[i].Value, layout);
                else
                {
                    instanceNotes.text = entries[i].Value;
                    RegisterText(instanceNotes);
                    if (layout != null)
                        scaledEntries.Add(new ScaledEntry { Layout = layout, Notes = instanceNotes });
                }
            }
        }

        ApplyAccessibilityScale(accessibilityScale);
    }

    private void OnDestroy()
    {
        if (eventDispatcher != null)
            eventDispatcher.RemoveListener<AccessibilityEvents.AccessibilityScaleUpdated>(onAccessibilityScaleUpdated);
    }

    private bool onAccessibilityScaleUpdated(AccessibilityEvents.AccessibilityScaleUpdated evt)
    {
        accessibilityScale = Mathf.Clamp(
            evt.Scale,
            AccessibilityConstants.SCALE_SMALL,
            AccessibilityConstants.SCALE_LARGE);
        ApplyAccessibilityScale(accessibilityScale);
        return false;
    }

    private void RegisterText(TMP_Text text)
    {
        if (text == null)
            return;

        scaledTexts.Add(new ScaledText { Text = text, BaseFontSize = text.fontSize });
    }

    private void ApplyAccessibilityScale(float scale)
    {
        scale = Mathf.Clamp(scale, AccessibilityConstants.SCALE_SMALL, AccessibilityConstants.SCALE_LARGE);
        foreach (ScaledText scaledText in scaledTexts)
        {
            if (scaledText.Text != null)
                scaledText.Text.fontSize = scaledText.BaseFontSize * scale;
        }

        foreach (ScaledEntry entry in scaledEntries)
            UpdateEntryLayout(entry);

        RectTransform layoutRoot = entryGameObject != null && entryGameObject.transform.parent != null
            ? entryGameObject.transform.parent as RectTransform
            : transform as RectTransform;
        if (layoutRoot != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }

    private void CreateNoteLines(Transform entryRoot, TMP_Text notesTemplate, string notes, LayoutElement entryLayout)
    {
        RectTransform notesRect = notesTemplate.rectTransform;
        GameObject containerObject = new GameObject("NotesLines", typeof(RectTransform), typeof(VerticalLayoutGroup));
        RectTransform container = containerObject.GetComponent<RectTransform>();
        container.SetParent(entryRoot, false);
        container.anchorMin = notesRect.anchorMin;
        container.anchorMax = notesRect.anchorMax;
        container.pivot = new Vector2(0.5f, 1f);
        container.anchoredPosition = new Vector2(notesRect.anchoredPosition.x, notesRect.anchoredPosition.y + notesRect.rect.height * 0.5f);
        container.sizeDelta = new Vector2(notesRect.rect.width, 0f);

        VerticalLayoutGroup verticalLayout = containerObject.GetComponent<VerticalLayoutGroup>();
        verticalLayout.childAlignment = TextAnchor.UpperLeft;
        verticalLayout.childControlWidth = true;
        verticalLayout.childControlHeight = true;
        verticalLayout.childForceExpandWidth = true;
        verticalLayout.childForceExpandHeight = false;
        verticalLayout.spacing = 10f;

        List<ScaledLine> lines = new List<ScaledLine>();
        string[] noteLines = (notes ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        notesTemplate.gameObject.SetActive(false);

        foreach (string noteLine in noteLines)
        {
            GameObject rowObject = new GameObject("NoteLine", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            RectTransform row = rowObject.GetComponent<RectTransform>();
            row.SetParent(container, false);
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = new Vector2(1f, 1f);
            row.pivot = new Vector2(0.5f, 1f);

            HorizontalLayoutGroup horizontalLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
            horizontalLayout.childAlignment = TextAnchor.UpperLeft;
            horizontalLayout.childControlWidth = true;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 6f;

            LayoutElement rowLayout = rowObject.GetComponent<LayoutElement>();
            rowLayout.flexibleWidth = 1f;

            GameObject symbolObject = Instantiate(lineSymbolText.gameObject, row, false);
            symbolObject.SetActive(true);
            TMP_Text symbol = symbolObject.GetComponent<TMP_Text>();
            LayoutElement symbolLayout = symbolObject.GetComponent<LayoutElement>();
            if (symbolLayout == null)
                symbolLayout = symbolObject.AddComponent<LayoutElement>();
            symbolLayout.ignoreLayout = false;
            symbolLayout.minWidth = symbol.GetPreferredValues(symbol.text).x;
            symbolLayout.preferredWidth = symbolLayout.minWidth;
            symbolLayout.flexibleWidth = 0f;
            RegisterText(symbol);

            GameObject notesObject = Instantiate(notesTemplate.gameObject, row, false);
            notesObject.SetActive(true);
            TMP_Text lineNotes = notesObject.GetComponent<TMP_Text>();
            lineNotes.text = noteLine;
            LayoutElement notesLayout = notesObject.GetComponent<LayoutElement>();
            if (notesLayout == null)
                notesLayout = notesObject.AddComponent<LayoutElement>();
            notesLayout.ignoreLayout = false;
            notesLayout.minWidth = 0f;
            notesLayout.preferredWidth = -1f;
            notesLayout.flexibleWidth = 1f;
            RegisterText(lineNotes);

            lines.Add(new ScaledLine
            {
                Row = row,
                Layout = rowLayout,
                SymbolLayout = symbolLayout,
                Symbol = symbol,
                Notes = lineNotes
            });
        }

        scaledEntries.Add(new ScaledEntry
        {
            Layout = entryLayout,
            LinesContainer = container,
            Lines = lines
        });
    }

    private static void UpdateEntryLayout(ScaledEntry entry)
    {
        if (entry.Layout == null)
            return;

        if (entry.Lines == null)
        {
            if (entry.Notes == null)
                return;

            entry.Notes.ForceMeshUpdate();
            float plainNotesHeight = entry.Notes.GetPreferredValues(entry.Notes.text, entry.Notes.rectTransform.rect.width, 0f).y;
            float plainNotesBottom = Mathf.Abs(entry.Notes.rectTransform.anchoredPosition.y) + plainNotesHeight;
            entry.Layout.minHeight = plainNotesBottom + 20f;
            entry.Layout.preferredHeight = plainNotesBottom + 20f;
            return;
        }

        float totalHeight = 0f;
        float availableWidth = entry.LinesContainer.rect.width;
        if (availableWidth <= 0f)
            availableWidth = entry.LinesContainer.sizeDelta.x;

        foreach (ScaledLine line in entry.Lines)
        {
            line.Symbol.ForceMeshUpdate();
            line.Notes.ForceMeshUpdate();
            float symbolWidth = line.Symbol.GetPreferredValues(line.Symbol.text).x;
            line.SymbolLayout.minWidth = symbolWidth;
            line.SymbolLayout.preferredWidth = symbolWidth;
            float notesWidth = Mathf.Max(1f, availableWidth - symbolWidth - 6f);
            float notesHeight = line.Notes.GetPreferredValues(line.Notes.text, notesWidth, 0f).y;
            float symbolHeight = line.Symbol.GetPreferredValues(line.Symbol.text, symbolWidth, 0f).y;
            float lineHeight = Mathf.Max(notesHeight, symbolHeight);
            line.Layout.minHeight = lineHeight;
            line.Row.sizeDelta = new Vector2(0f, lineHeight);
            totalHeight += lineHeight;
        }

        totalHeight += Mathf.Max(0, entry.Lines.Count - 1) * 10f;
        entry.LinesContainer.sizeDelta = new Vector2(entry.LinesContainer.sizeDelta.x, totalHeight);
        LayoutRebuilder.ForceRebuildLayoutImmediate(entry.LinesContainer);
        float bottom = Mathf.Abs(entry.LinesContainer.anchoredPosition.y) + totalHeight;
        entry.Layout.minHeight = bottom + 20f;
        entry.Layout.preferredHeight = bottom + 20f;
    }

    private static string GetPlatformKey(string key)
    {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        if (Application.isEditor)
            return "Editor_" + key;
#endif
        return key;
    }

    private static TMP_Text FindText(Transform root, string path)
    {
        Transform target = path == "." ? root : root.Find(path);
        return target != null ? target.GetComponent<TMP_Text>() : null;
    }

    private static void ApplyImageColor(Transform root, string path, Color color)
    {
        if (path == null)
            return;

        Transform target = path == "." ? root : root.Find(path);
        if (target == null)
            return;

        Image image = target.GetComponent<Image>();
        if (image == null)
            image = target.GetComponentInChildren<Image>(true);
        if (image != null)
            image.color = color;
    }

    private static string GetRelativePath(Transform root, Transform target)
    {
        if (target == null)
            return null;
        if (root == target)
            return ".";

        string path = target.name;
        Transform current = target.parent;
        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return current == root ? path : null;
    }

    private static List<KeyValuePair<string, string>> ParseEntries(string json)
    {
        List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>();
        int position = 0;
        while (position < json.Length)
        {
            SkipSeparators(json, ref position);
            if (position >= json.Length || json[position] == '}')
                break;

            string date = ReadString(json, ref position);
            SkipWhitespace(json, ref position);
            if (position >= json.Length || json[position++] != ':')
                break;
            SkipWhitespace(json, ref position);
            string notes = ReadString(json, ref position);
            entries.Add(new KeyValuePair<string, string>(date, notes));
        }
        return entries;
    }

    private static void SkipSeparators(string json, ref int position)
    {
        while (position < json.Length && (char.IsWhiteSpace(json[position]) || json[position] == '{' || json[position] == ','))
            position++;
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
}
