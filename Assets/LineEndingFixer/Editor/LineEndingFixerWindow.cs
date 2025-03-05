using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System;

namespace Kamgam.LEF
{
    public static class BackgroundStyle
    {
        private static GUIStyle style = new GUIStyle();
        private static Texture2D texture;

        public static GUIStyle Get(Color color)
        {
            if (texture == null)
                texture = new Texture2D(1, 1);

            texture.SetPixel(0, 0, color);
            texture.Apply();
            style.normal.background = texture;

            return style;
        }
    }

    public class LineEndingFixerWindow : EditorWindow
    {
        public static GUIStyle ErrorGroupDetailsButtonBoxStyle;
        public static Color WarningTextColor = new Color(0.9f, 0.2f, 0.2f);

        private static Color _DefaultBackgroundColor;
        public static Color DefaultBackgroundColor
        {
            get
            {
                if (_DefaultBackgroundColor.a == 0)
                {
                    try
                    {
                        var method = typeof(EditorGUIUtility).GetMethod("GetDefaultBackgroundColor", BindingFlags.NonPublic | BindingFlags.Static);
                        _DefaultBackgroundColor = (Color)method.Invoke(null, null);
                    }
                    catch
                    {
                        // fallback if reflection fails
                        _DefaultBackgroundColor = new Color32(56, 56, 56, 255);
                    }
                }
                return _DefaultBackgroundColor;
            }
        }

        protected bool behaviourInfoFoldout = false;

        protected Vector2 scanResultsScrollViewPos;

        protected bool isScanning;
        protected Vector2 scanLogScrollPos = Vector2.zero;
        protected bool scanLogsFoldout = false;
        protected bool scanWasCancelled = false;
        protected List<string> scanLogs = new List<string>();
        protected List<ScriptFile> scanResults;
        protected List<ScriptFile> fixedFiles = new List<ScriptFile>();
        protected System.Threading.CancellationTokenSource scanTokenSource;

        [MenuItem("Window/Line Ending Fixer")]
        static LineEndingFixerWindow openWindow()
        {
            LineEndingFixerWindow window = (LineEndingFixerWindow)EditorWindow.GetWindow(typeof(LineEndingFixerWindow));
            window.titleContent = new GUIContent("Line Ending Fixer");
            window.Initialize();
            window.Show();
            return window;
        }

        public static LineEndingFixerWindow GetOrOpen()
        {
            if (!HasOpenInstances<LineEndingFixerWindow>())
            {
                var window = openWindow();
                window.Focus();
                return window;
            }
            else
            {
                var window = GetWindow<LineEndingFixerWindow>();
                window.Focus();
                return window;
            }
        }

        public void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            ErrorGroupDetailsButtonBoxStyle = BackgroundStyle.Get(DefaultBackgroundColor);

            if (!isDocked())
            {
                if (position.width < 600 || position.height < 200)
                {
                    const int width = 900;
                    const int height = 700;
                    var x = Screen.currentResolution.width / 2 - width;
                    var y = Screen.currentResolution.height / 2 - height;
                    position = new Rect(x, y, width, height);
                }
            }

            isScanning = false;

            _bannerBgStyle.normal.background = makeTex(4, 4, new Color(0f, 0f, 0f, 1f));
        }

        GUIStyle _bannerBgStyle = new GUIStyle();

        private Texture2D makeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        protected bool isDocked()
        {
#if UNITY_2020_1_OR_NEWER
            return docked;
#else
            return true;
#endif
        }

        void OnGUI()
        {
            try
            {
                
                // Old school banner ad :D
                GUILayout.Space(8);
                var bgColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(bgColor.r * 0.7f, bgColor.g * 0.7f, bgColor.b * 0.7f, 1f);
                GUILayout.BeginHorizontal(_bannerBgStyle);
                DrawLabel("Created by " + 
                    "<color=#a02627>K</color>" +
                    "<color=#f1552c>A</color>" +
                    "<color=#f98238>M</color>" +
                    "<color=#fbc43e>G</color>" +
                    "<color=#ffeb49>A</color>" +
                    "<color=#ffeb49>M</color> -> "
                    , null, true, false, true);
                var color = GUI.color;
                GUI.color = new Color(0.976f ,0.51f, 0.22f);
                if (DrawButton("Please check out my other assets."))
                {
                    Application.OpenURL("https://kamgam.com/unity");
                }
                GUI.color = color;
                DrawLabel(" or ");
                if (DrawButton("Leave a review."))
                {
                    Application.OpenURL("https://assetstore.unity.com/packages/slug/229464");
                }
                DrawLabel("Thanks!", wordwrap: false);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUI.backgroundColor = bgColor;

                GUILayout.Space(8);

                // Title bar with buttons
                GUILayout.BeginHorizontal();
                DrawLabel("Line Ending Settings", bold: true, options: GUILayout.MinWidth(150));

                GUILayout.FlexibleSpace();
                GUILayout.Label("Version " + LineEndingFixerSettings.Version + " ");
                if (DrawButton(" Manual ", icon: "_Help"))
                {
                    OpenManual();
                }
                if (DrawButton(" Settings ", icon: "_Popup"))
                {
                    OpenSettings();
                }
                GUILayout.EndHorizontal();

                ////////////////////////////////////


                // Settings

                var settings = LineEndingFixerSettings.GetOrCreateSettings();
                settings.LineEnding = (LineEndingBehaviour) EditorGUILayout.EnumPopup(new GUIContent("Line Ending Behaviour:", LineEndingFixerSettings._LineEndingTooltip), settings.LineEnding, GUILayout.MaxWidth(350) );

                GUILayout.BeginVertical(EditorStyles.helpBox);
                behaviourInfoFoldout = EditorGUILayout.Foldout(behaviourInfoFoldout, "Info");
                if (behaviourInfoFoldout)
                {
                    DrawLabel(LineEndingFixerSettings._LineEndingTooltip);
                }
                GUILayout.EndVertical();
                

                ////////////////////////////////////



                // Scan 

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                DrawLabel("Scan Scripts", bold: true);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(isScanning ? "Stop Scanning .." : " Start Scan "))
                {
                    if (!isScanning)
                    {
                        StartScanning();
                    }
                    else
                    {
                        StopScanning();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                DrawLabel("Scans through all scripts and checks if any of them have mixed line endings.", wordwrap: true);
                
                // scan results
                DrawScanResults();
                GUILayout.FlexibleSpace();

                GUILayout.EndVertical();

                ////////////////////////////////////
            }
            catch
            {
                isScanning = false;
                scanResults = null;
                fixedFiles.Clear();
                scanLogs.Clear();
                if (scanTokenSource != null)
                {
                    scanTokenSource.Cancel();
                    scanTokenSource = null;
                }
                throw;
            }
        }

        private void DrawScanResults()
        {
            // logs
            if (scanLogs.Count > 0)
            {
                scanLogsFoldout = EditorGUILayout.Foldout(scanLogsFoldout || isScanning, "Logs");
                if (scanLogsFoldout)
                {
                    scanLogScrollPos = GUILayout.BeginScrollView(isScanning ? new Vector2(0, scanLogs.Count * 17) : scanLogScrollPos, GUILayout.Height(200));
                    foreach (var line in scanLogs)
                    {
                        DrawLabel(line, richText: false);
                    }
                    GUILayout.EndScrollView();
                }
            }

            if (scanResults == null)
            {
                return;
            }


            GUILayout.Space(10);

            // show list of excluded folders
            DrawLabel("<b>Scan Results</b>");

            if (scanResults.Count == 0)
            {
                DrawLabel("No scripts with mixed line endings found unter /Assets");
                return;
            }

            var settings = LineEndingFixerSettings.GetOrCreateSettings();
            scanResultsScrollViewPos = GUILayout.BeginScrollView(scanResultsScrollViewPos);
            foreach (var scriptFile in scanResults)
            {
                GUILayout.BeginHorizontal();
                string fileName = Path.GetFileName(scriptFile.Path);
                string filePath = scriptFile.Path.Replace(fileName, "");
                filePath = filePath.Replace("/", " / ");
                DrawLabel(WrapInRichTextColor(filePath, new Color(0.6f, 0.6f, 0.6f)) + fileName, wordwrap: true, icon: scriptFile.MiniThumbnail);

                GUI.enabled = !fixedFiles.Contains(scriptFile);
                if (GUILayout.Button(" Fix Line Endings ", GUILayout.Width(120)))
                {
                    SingleFileFixer.FixLineEndings(scriptFile.Path, settings.LineEnding, settings.DefaultEncoding);
                    fixedFiles.Add(scriptFile);
                }

                GUI.enabled = true;
                if (DrawButton(" Go To ", icon: "Animation.Play", options: GUILayout.Width(70)))
                {
                    var path = AssetDatabase.GUIDToAssetPath(scriptFile.GUID);
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    EditorGUIUtility.PingObject(obj);
                }
                GUILayout.EndHorizontal();
            }

            // fix all
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = scanResults != null && (scanResults != null && scanResults.Count != fixedFiles.Count);
            if (DrawButton(" Fix all scripts ", options: GUILayout.Width(250)))
            {
                bool fixAll = EditorUtility.DisplayDialog("Are you sure?", "This will fix all the files listed above.\n\nNOTICE: You can not UNDO this!", "Yes, fix all", "Canel");
                if (fixAll)
                {
                    foreach (var scriptFile in scanResults)
                    {
                        SingleFileFixer.FixLineEndings(scriptFile.Path, settings.LineEnding, settings.DefaultEncoding);
                        fixedFiles.Add(scriptFile);
                    }
                }
            }
            GUI.enabled= true;
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        public static void OpenManual()
        {
            EditorUtility.OpenWithDefaultApp("Assets/LineEndingFixer/LineEndingFixerManual.pdf");
        }

        public void OpenSettings()
        {
            LineEndingFixerSettings.SelectSettings();
        }

        public async void StartScanning()
        {
            if (scanTokenSource != null && !scanTokenSource.IsCancellationRequested)
                scanTokenSource.Cancel();

            isScanning = true;
            scanResults = null;
            fixedFiles.Clear();
            scanLogs.Clear();
            scanLogScrollPos = Vector2.zero;
            scanLogsFoldout = false;
            scanWasCancelled = false;

            try
            {
                scanTokenSource = new System.Threading.CancellationTokenSource();
                var ct = scanTokenSource.Token;

                await Task.Delay(100);
                scanResults = await LineEndingFixer.ScanForBrokenScripts(ct, scanLogs);
                Repaint();
            }
            finally
            {
                isScanning = false;
                scanLogsFoldout = scanWasCancelled;
            }
        }

        public void StopScanning()
        {
            if (scanTokenSource != null && !scanTokenSource.IsCancellationRequested)
                scanTokenSource.Cancel();

            scanWasCancelled = true;
        }

        #region Utilitiy Functions

        public static void DrawLabel(string text, Color? color = null, bool bold = false, bool wordwrap = true, bool richText = true, Texture icon = null, params GUILayoutOption[] options)
        {
            if (!color.HasValue)
                color = GUI.skin.label.normal.textColor;

            var style = new GUIStyle(GUI.skin.label);
            if (bold)
                style.fontStyle = FontStyle.Bold;

            style.normal.textColor = color.Value;
            style.wordWrap = wordwrap;
            style.richText = richText;
            style.imagePosition = ImagePosition.ImageLeft;

            var content = new GUIContent();
            content.text = text;
            if (icon != null)
            {
                GUILayout.Space(16);
                var position = GUILayoutUtility.GetRect(content, style);
                GUI.DrawTexture(new Rect(position.x - 16, position.y, 16, 16), icon);
                GUI.Label(position, content, style);
            }
            else
            {
                GUILayout.Label(text, style, options);
            }
        }

        public static void DrawSelectableLabel(string text, Color? color = null, bool bold = false, bool wordwrap = true, bool richText = true)
        {
            if (!color.HasValue)
                color = GUI.skin.label.normal.textColor;

            var style = new GUIStyle(GUI.skin.label);
            if (bold)
                style.fontStyle = FontStyle.Bold;
            style.normal.textColor = color.Value;
            style.wordWrap = wordwrap;
            style.richText = richText;

            var content = new GUIContent(text);
            var position = GUILayoutUtility.GetRect(content, style);
            EditorGUI.SelectableLabel(position, text, style);
        }

        public static bool DrawButton(string text, string tooltip = null, string icon = null, params GUILayoutOption[] options)
        {
            GUIContent content;

            // icon
            if (!string.IsNullOrEmpty(icon))
                content = EditorGUIUtility.IconContent(icon);
            else
                content = new GUIContent();

            // text
            content.text = text;

            // tooltip
            if (!string.IsNullOrEmpty(tooltip))
                content.tooltip = tooltip;

            return GUILayout.Button(content, options);
        }

        public static void BeginHorizontalIndent(int indentAmount = 10, bool beginVerticalInside = true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentAmount);
            if (beginVerticalInside)
                GUILayout.BeginVertical();
        }

        public static void EndHorizontalIndent(float indentAmount = 10, bool begunVerticalInside = true, bool bothSides = false)
        {
            if (begunVerticalInside)
                GUILayout.EndVertical();
            if (bothSides)
                GUILayout.Space(indentAmount);
            GUILayout.EndHorizontal();
        }

        public static string WrapInRichTextColor(string text, Color color)
        {
            var hexColor = ColorUtility.ToHtmlStringRGB(color);
            return "<color=#" + hexColor + ">" + text + "</color>";
        }

        #endregion
    }
}
