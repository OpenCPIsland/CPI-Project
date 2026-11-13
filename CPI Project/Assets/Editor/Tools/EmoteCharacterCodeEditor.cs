using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ClubPenguin.Editor
{
    public class EmoteCharacterCodeEditor : EditorWindow
    {
        private const string EmojiTexturePath = "Assets/Game/UI/Common/Fonts/ProximaNovaBitmap/ProximaNova_Semibold_bitmap_emoji.png";
        private const string FontAssetPath = "Assets/Game/UI/Common/Fonts/ProximaNovaBitmap/ProximaNova_Semibold_bitmap.asset";
        private const string EmoteDefinitionsPath = "Assets/Game/UI/ChatEmotes/Definitions/EmoteDefinitions";

        private struct CellInfo
        {
            public int CharacterCode;
            public string EmoteName;
            public int PixelX, PixelY, PixelW, PixelH;
            public Texture2D Thumbnail;
        }

        private Texture2D emojiTexture;
        private Texture2D readableTexture;
        private List<CellInfo> cells = new List<CellInfo>();
        private Dictionary<int, string> codeToEmoteName = new Dictionary<int, string>();

        private Vector2 scrollPosition;
        private int selectedCellIndex = -1;
        private float zoomLevel = 1.5f;
        private Texture2D replacementTexture;

        private bool showList = false;
        private Vector2 listScrollPosition;
        private string searchFilter = "";
        private int copiedCellIndex = -1;

        [MenuItem("Project/Tools/Emote Character Code Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<EmoteCharacterCodeEditor>("Emoji Sprite Editor");
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void OnDisable()
        {
            CleanupThumbnails();
            if (readableTexture != null)
                DestroyImmediate(readableTexture);
        }

        private void CleanupThumbnails()
        {
            foreach (var cell in cells)
            {
                if (cell.Thumbnail != null)
                    DestroyImmediate(cell.Thumbnail);
            }
        }

        private void LoadData()
        {
            CleanupThumbnails();

            emojiTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(EmojiTexturePath);
            if (emojiTexture == null) return;

            if (readableTexture != null)
                DestroyImmediate(readableTexture);
            readableTexture = MakeReadable(emojiTexture);

            codeToEmoteName.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { EmoteDefinitionsPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (so == null || so.GetType().Name != "EmoteDefinition") continue;
                var serialized = new SerializedObject(so);
                var codeProp = serialized.FindProperty("CharacterCode");
                var idProp = serialized.FindProperty("Id");
                if (codeProp != null && idProp != null)
                    codeToEmoteName[codeProp.intValue] = idProp.stringValue;
            }

            ParseFontAssetCells();
        }

        private void ParseFontAssetCells()
        {
            cells.Clear();
            string fontAssetFullPath = Path.Combine(Application.dataPath, "..", FontAssetPath);
            if (!File.Exists(fontAssetFullPath)) return;

            string content = File.ReadAllText(fontAssetFullPath);
            var regex = new Regex(
                @"index:\s+(57\d+)\s+uv:\s+serializedVersion:\s+2\s+x:\s+([^\r\n]+)\s+y:\s+([^\r\n]+)\s+width:\s+([^\r\n]+)\s+height:\s+([^\r\n]+)",
                RegexOptions.Multiline);

            int texW = readableTexture.width;
            int texH = readableTexture.height;

            foreach (Match m in regex.Matches(content))
            {
                int code = int.Parse(m.Groups[1].Value);
                float uvX = float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                float uvY = float.Parse(m.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
                float uvW = float.Parse(m.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture);
                float uvH = float.Parse(m.Groups[5].Value, System.Globalization.CultureInfo.InvariantCulture);

                string name = "";
                codeToEmoteName.TryGetValue(code, out name);

                float emojiUvX = (uvX - 0.5f) * 2.0f;
                float emojiUvW = uvW * 2.0f;
                float emojiUvY = uvY;
                float emojiUvH = uvH;

                int pxX = Mathf.RoundToInt(emojiUvX * texW);
                int pxY = Mathf.RoundToInt(emojiUvY * texH);
                int pxW = Mathf.RoundToInt(emojiUvW * texW);
                int pxH = Mathf.RoundToInt(emojiUvH * texH);

                pxX = Mathf.Clamp(pxX, 0, texW - 1);
                pxY = Mathf.Clamp(pxY, 0, texH - 1);
                pxW = Mathf.Clamp(pxW, 1, texW - pxX);
                pxH = Mathf.Clamp(pxH, 1, texH - pxY);

                Color[] pixels = readableTexture.GetPixels(pxX, pxY, pxW, pxH);
                Texture2D thumb = new Texture2D(pxW, pxH, TextureFormat.RGBA32, false);
                thumb.filterMode = FilterMode.Trilinear;
                thumb.anisoLevel = 16;
                thumb.SetPixels(pixels);
                thumb.Apply();

                cells.Add(new CellInfo
                {
                    CharacterCode = code,
                    EmoteName = name ?? "",
                    PixelX = pxX,
                    PixelY = pxY,
                    PixelW = pxW,
                    PixelH = pxH,
                    Thumbnail = thumb
                });
            }

            cells.Sort((a, b) => a.CharacterCode.CompareTo(b.CharacterCode));
            Debug.Log($"[EmojiEditor] Loaded {cells.Count} emoji cells from {texW}x{texH} texture");
        }

        private Texture2D MakeReadable(Texture2D source)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            Graphics.Blit(source, tmp);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            readable.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(tmp);
            return readable;
        }

        private void OnGUI()
        {
            if (emojiTexture == null)
            {
                EditorGUILayout.HelpBox($"Could not load emoji texture at:\n{EmojiTexturePath}", MessageType.Error);
                if (GUILayout.Button("Retry")) LoadData();
                return;
            }

            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawSpriteGrid();
            DrawDetailsPanel();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField($"Emoji Sprite Editor  |  {cells.Count} emojis  |  {emojiTexture.width}x{emojiTexture.height}px", EditorStyles.miniLabel, GUILayout.Width(300));

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Zoom:", GUILayout.Width(40));
            zoomLevel = GUILayout.HorizontalSlider(zoomLevel, 0.5f, 4.0f, GUILayout.Width(100));
            EditorGUILayout.LabelField($"{zoomLevel:F1}x", GUILayout.Width(30));

            showList = GUILayout.Toggle(showList, "List View", EditorStyles.toolbarButton, GUILayout.Width(70));

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                LoadData();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpriteGrid()
        {
            float panelWidth = showList ? position.width * 0.55f : position.width - 280;

            EditorGUILayout.BeginVertical(GUILayout.Width(panelWidth));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (cells.Count == 0)
            {
                EditorGUILayout.HelpBox("No emoji cells found in the font asset.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                return;
            }

            float cellDisplayW = cells[0].PixelW * zoomLevel;
            float cellDisplayH = cells[0].PixelH * zoomLevel;
            float cellTotalH = cellDisplayH + 14;
            float cellTotalW = cellDisplayW + 4;
            int colsPerRow = Mathf.Max(1, Mathf.FloorToInt((panelWidth - 20) / cellTotalW));

            int col = 0;
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                bool isSelected = (selectedCellIndex == i);

                Rect btnRect = GUILayoutUtility.GetRect(cellTotalW, cellTotalH, GUILayout.Width(cellTotalW), GUILayout.Height(cellTotalH));

                if (isSelected)
                    EditorGUI.DrawRect(btnRect, new Color(0.2f, 0.5f, 1f, 0.4f));

                Rect imageRect = new Rect(btnRect.x + 2, btnRect.y, cellDisplayW, cellDisplayH);
                if (cell.Thumbnail != null)
                    GUI.DrawTexture(imageRect, cell.Thumbnail, ScaleMode.StretchToFill);

                Rect labelRect = new Rect(btnRect.x, btnRect.y + cellDisplayH, cellTotalW, 14);
                GUI.Label(labelRect, cell.CharacterCode.ToString(), EditorStyles.centeredGreyMiniLabel);

                if (Event.current.type == EventType.MouseDown && btnRect.Contains(Event.current.mousePosition))
                {
                    selectedCellIndex = i;
                    Event.current.Use();
                    Repaint();
                }

                col++;
                if (col >= colsPerRow)
                {
                    col = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawDetailsPanel()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(showList ? position.width * 0.45f : 276));

            if (showList)
                DrawListView();
            else
                DrawCellDetails();

            EditorGUILayout.EndVertical();
        }

        private void DrawCellDetails()
        {
            EditorGUILayout.LabelField("Selected Emoji", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            if (selectedCellIndex < 0 || selectedCellIndex >= cells.Count)
            {
                EditorGUILayout.HelpBox("Click an emoji in the grid to select it.", MessageType.Info);
                return;
            }

            var cell = cells[selectedCellIndex];

            float previewSize = 128;
            Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize, GUILayout.Width(previewSize), GUILayout.Height(previewSize));
            EditorGUI.DrawRect(previewRect, new Color(0.15f, 0.15f, 0.15f, 1));
            if (cell.Thumbnail != null)
                GUI.DrawTexture(previewRect, cell.Thumbnail, ScaleMode.ScaleToFit);

            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField("Character Code:", cell.CharacterCode.ToString());
            EditorGUILayout.LabelField("Hex Code:", $"0x{cell.CharacterCode:X4}");
            EditorGUILayout.LabelField("Emote Name:", string.IsNullOrEmpty(cell.EmoteName) ? "(unmapped)" : cell.EmoteName);
            EditorGUILayout.LabelField("Pixel Rect:", $"({cell.PixelX}, {cell.PixelY})  {cell.PixelW}x{cell.PixelH}");

            if (GUILayout.Button("Copy Info"))
                EditorGUIUtility.systemCopyBuffer = $"{cell.EmoteName}\t{cell.CharacterCode}\t0x{cell.CharacterCode:X4}";

            EditorGUILayout.Space(12);

            EditorGUILayout.LabelField("Copy Image to Another Emoji", EditorStyles.boldLabel);

            if (GUILayout.Button("Copy Image", GUILayout.Height(24)))
            {
                copiedCellIndex = selectedCellIndex;
            }

            if (copiedCellIndex >= 0 && copiedCellIndex < cells.Count)
            {
                var srcCell = cells[copiedCellIndex];
                string srcLabel = !string.IsNullOrEmpty(srcCell.EmoteName) ? srcCell.EmoteName : srcCell.CharacterCode.ToString();
                EditorGUILayout.LabelField("Copied:", srcLabel);

                if (srcCell.Thumbnail != null)
                {
                    Rect copyPreview = GUILayoutUtility.GetRect(48, 48, GUILayout.Width(48), GUILayout.Height(48));
                    EditorGUI.DrawRect(copyPreview, new Color(0.15f, 0.15f, 0.15f, 1));
                    GUI.DrawTexture(copyPreview, srcCell.Thumbnail, ScaleMode.ScaleToFit);
                }

                bool isSameCell = copiedCellIndex == selectedCellIndex;
                EditorGUI.BeginDisabledGroup(isSameCell);
                if (GUILayout.Button("Paste Image Here", GUILayout.Height(24)))
                    CopyEmojiCell(copiedCellIndex, selectedCellIndex);
                EditorGUI.EndDisabledGroup();

                if (isSameCell)
                    EditorGUILayout.HelpBox("Select a different emoji to paste onto.", MessageType.Info);
            }

            EditorGUILayout.Space(12);

            EditorGUILayout.LabelField("Replace This Emoji", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Drag a replacement PNG below to overwrite this emoji's pixels on the sprite sheet.",
                MessageType.Info);
            EditorGUILayout.Space(4);

            replacementTexture = (Texture2D)EditorGUILayout.ObjectField("Replacement PNG:", replacementTexture, typeof(Texture2D), false);

            if (replacementTexture != null)
            {
                Rect replRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUI.DrawRect(replRect, new Color(0.15f, 0.15f, 0.15f, 1));
                GUI.DrawTexture(replRect, replacementTexture, ScaleMode.ScaleToFit);
                EditorGUILayout.Space(4);
            }

            EditorGUI.BeginDisabledGroup(replacementTexture == null);
            if (GUILayout.Button("Replace Emoji on Sprite Sheet", GUILayout.Height(30)))
                ReplaceEmojiCell(selectedCellIndex, replacementTexture);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawListView()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(34));
            GUILayout.Label("Code", EditorStyles.miniLabel, GUILayout.Width(55));
            GUILayout.Label("Hex", EditorStyles.miniLabel, GUILayout.Width(60));
            GUILayout.Label("Name", EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];

                if (!string.IsNullOrEmpty(searchFilter))
                {
                    bool match = cell.CharacterCode.ToString().Contains(searchFilter)
                        || cell.EmoteName.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0
                        || $"0x{cell.CharacterCode:X4}".IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!match) continue;
                }

                Rect rowRect = EditorGUILayout.BeginHorizontal();
                if (selectedCellIndex == i)
                    EditorGUI.DrawRect(rowRect, new Color(0.2f, 0.5f, 1f, 0.3f));

                Rect thumbRect = GUILayoutUtility.GetRect(30, 30, GUILayout.Width(30), GUILayout.Height(30));
                if (cell.Thumbnail != null)
                    GUI.DrawTexture(thumbRect, cell.Thumbnail, ScaleMode.ScaleToFit);

                GUILayout.Label(cell.CharacterCode.ToString(), EditorStyles.miniLabel, GUILayout.Width(55));
                GUILayout.Label($"0x{cell.CharacterCode:X4}", EditorStyles.miniLabel, GUILayout.Width(60));
                GUILayout.Label(cell.EmoteName, EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    selectedCellIndex = i;
                    showList = false;
                    Event.current.Use();
                    Repaint();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void CopyEmojiCell(int srcIndex, int dstIndex)
        {
            if (srcIndex < 0 || srcIndex >= cells.Count || dstIndex < 0 || dstIndex >= cells.Count) return;

            var src = cells[srcIndex];
            var dst = cells[dstIndex];

            string srcLabel = !string.IsNullOrEmpty(src.EmoteName) ? src.EmoteName : src.CharacterCode.ToString();
            string dstLabel = !string.IsNullOrEmpty(dst.EmoteName) ? dst.EmoteName : dst.CharacterCode.ToString();

            string msg = $"Copy image from {srcLabel} ({src.CharacterCode}) to {dstLabel} ({dst.CharacterCode})";
            msg += $"\nSource rect: ({src.PixelX}, {src.PixelY}) {src.PixelW}x{src.PixelH}";
            msg += $"\nTarget rect: ({dst.PixelX}, {dst.PixelY}) {dst.PixelW}x{dst.PixelH}";

            if (!EditorUtility.DisplayDialog("Confirm Copy", msg, "Copy", "Cancel"))
                return;

            Color[] srcPixels = readableTexture.GetPixels(src.PixelX, src.PixelY, src.PixelW, src.PixelH);

            if (src.PixelW != dst.PixelW || src.PixelH != dst.PixelH)
            {
                Texture2D srcTex = new Texture2D(src.PixelW, src.PixelH, TextureFormat.RGBA32, false);
                srcTex.SetPixels(srcPixels);
                srcTex.Apply();
                Texture2D scaled = ScaleTexture(srcTex, dst.PixelW, dst.PixelH);
                srcPixels = scaled.GetPixels();
                DestroyImmediate(srcTex);
                DestroyImmediate(scaled);
            }

            readableTexture.SetPixels(dst.PixelX, dst.PixelY, dst.PixelW, dst.PixelH, srcPixels);
            readableTexture.Apply();

            string fullPath = Path.Combine(Application.dataPath, "..", EmojiTexturePath);
            byte[] pngBytes = readableTexture.EncodeToPNG();
            File.WriteAllBytes(fullPath, pngBytes);

            AssetDatabase.ImportAsset(EmojiTexturePath, ImportAssetOptions.ForceUpdate);
            LoadData();

            Debug.Log($"[EmojiEditor] Copied image from {srcLabel} ({src.CharacterCode}) to {dstLabel} ({dst.CharacterCode})");
            EditorUtility.DisplayDialog("Done", $"Copied image from {srcLabel} to {dstLabel}.", "OK");
        }

        private void ReplaceEmojiCell(int cellIndex, Texture2D replacement)
        {
            if (cellIndex < 0 || cellIndex >= cells.Count || replacement == null) return;

            var cell = cells[cellIndex];

            string msg = $"Replace emoji {cell.CharacterCode}";
            if (!string.IsNullOrEmpty(cell.EmoteName)) msg += $" ({cell.EmoteName})";
            msg += $"\nPixel rect: ({cell.PixelX}, {cell.PixelY}) {cell.PixelW}x{cell.PixelH}";
            msg += "\n\nA backup of the sprite sheet will be created.";

            if (!EditorUtility.DisplayDialog("Confirm Replace", msg, "Replace", "Cancel"))
                return;

            string fullPath = Path.Combine(Application.dataPath, "..", EmojiTexturePath);

            Texture2D readableReplacement = MakeReadable(replacement);
            Texture2D scaled = ScaleTexture(readableReplacement, cell.PixelW, cell.PixelH);

            Color[] srcPixels = scaled.GetPixels();
            readableTexture.SetPixels(cell.PixelX, cell.PixelY, cell.PixelW, cell.PixelH, srcPixels);
            readableTexture.Apply();

            byte[] pngBytes = readableTexture.EncodeToPNG();
            File.WriteAllBytes(fullPath, pngBytes);

            DestroyImmediate(readableReplacement);
            DestroyImmediate(scaled);

            AssetDatabase.ImportAsset(EmojiTexturePath, ImportAssetOptions.ForceUpdate);

            LoadData();

            Debug.Log($"[EmojiEditor] Replaced emoji {cell.CharacterCode} ({cell.EmoteName})");
            EditorUtility.DisplayDialog("Done", $"Emoji {cell.CharacterCode} replaced.", "OK");
        }

        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            tmp.filterMode = FilterMode.Trilinear;
            tmp.anisoLevel = 16;
            Graphics.Blit(source, tmp);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            result.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(tmp);
            return result;
        }
    }
}
