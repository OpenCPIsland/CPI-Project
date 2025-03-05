using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ASoliman.Utils.ClipboardPlus
{
    /// <summary>
    /// A custom editor window for handling clipboard operations in Unity,  
    /// enabling seamless copying, pasting, and organization of component references.
    /// </summary>
    public class ClipboardEditorWindow : EditorWindow
    {
        private const int WINDOW_PADDING = 2;
        private readonly Color _hoverPasteColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        private readonly Color _hoverRemoveColor = new Color(0.8f, 0.3f, 0.3f, 1f);

        // UI-related fields
        private Vector2 _scrollPosition;
        private string _searchQuery = "";
        private bool _showFavoritesOnly = false;
        private bool _isTypeFiltered = false;

        // State-related fields
        private Object _pasteTarget;
        private Component _selectedComponent;
        private bool _stylesInitialized = false;

        // GUIStyle fields
        private GUIStyle _clipStyle;
        private GUIStyle _targetIndicatorStyle;
        private GUIStyle _closeButtonStyle;
        private GUIStyle _filterLabelStyle;

        [MenuItem("Window/Clipboard Plus &w")]
        public static ClipboardEditorWindow ShowWindow()
        {
            var window = GetWindow<ClipboardEditorWindow>("Clipboard");
            window.minSize = new Vector2(300, 200);
            var icon = Resources.Load<Texture2D>("clipboard");
            window.titleContent.image = icon;
            return window;
        }

        private void OnEnable()
        {
            _stylesInitialized = false;

            if (ClipboardManager.Instance != null)
            {
                ClipboardManager.Instance.ValidateClips();
            }

            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
            _stylesInitialized = false;
        }

        private void OnGUI()
        {
            if (!_stylesInitialized)
            {
                InitializeStyles();
                if (!_stylesInitialized)
                {
                    EditorGUILayout.HelpBox("Initializing styles...", MessageType.Info);
                    return;
                }
            }

            DrawSelectedComponentIndicator();
            HandleDragAndDrop();

            if (_selectedComponent != null)
            {
                DrawTopBorder();
            }

            DrawToolbar();
            DrawTypeFilterIndicator();
            DrawClipList();
        }

        private void OnSelectionChange()
        {
            _selectedComponent = Selection.activeObject as Component;
            Repaint();
        }

        /// <summary>
        /// Forces all ClipboardEditorWindow instances to repaint.
        /// This is useful when the clipboard data changes outside of the window's normal update cycle.
        /// </summary>
        public static void ForceRepaint()
        {
            var windows = Resources.FindObjectsOfTypeAll<ClipboardEditorWindow>();
            foreach (var window in windows)
            {
                if (window != null)
                {
                    window.Repaint();
                }
            }
        }

        /// <summary>
        /// Sets the target object for paste operations and updates the window's state accordingly.
        /// </summary>
        /// <param name="target">The Unity Object to set as the paste target</param>
        public void SetPasteTarget(Object target)
        {
            _pasteTarget = target;
            _selectedComponent = null;

            if (target is Component component)
            {
                _selectedComponent = component;
                _isTypeFiltered = true;
            }
        }

        private void InitializeStyles()
        {
            if (EditorStyles.helpBox == null) return;

            try
            {
                InitializeClipStyle();
                InitializeTargetIndicatorStyle();
                InitializeCloseButtonStyle();
                InitializeFilterLabelStyle();
                _stylesInitialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to initialize styles: {e.Message}");
                _stylesInitialized = false;
            }
        }

        private void InitializeClipStyle()
        {
            _clipStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
        }

        private void InitializeTargetIndicatorStyle()
        {
            _targetIndicatorStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(0, 0, WINDOW_PADDING, 0),
                fontSize = 11,
                border = new RectOffset(1, 1, 1, 1),
                stretchWidth = false,
                fixedHeight = 26
            };
            _targetIndicatorStyle.normal.textColor = EditorGUIUtility.isProSkin ?
                new Color(0.9f, 0.9f, 0.9f) :
                new Color(0.2f, 0.2f, 0.2f);
        }

        private void InitializeCloseButtonStyle()
        {
            _closeButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 1),
                margin = new RectOffset(4, 0, 7, 0),
                fixedWidth = 16,
                fixedHeight = 16
            };
        }

        private void InitializeFilterLabelStyle()
        {
            _filterLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(6, 6, 2, 2),
                fontSize = 10,
                alignment = TextAnchor.MiddleLeft,
                normal = { background = CreateBadgeTexture() }
            };
            _filterLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ?
                new Color(0.8f, 0.8f, 0.8f) :
                new Color(0.3f, 0.3f, 0.3f);
        }

        /// <summary>
        /// Creates a badge-style background texture for filter labels.
        /// </summary>
        /// <returns>A single pixel texture used for filter label backgrounds</returns>
        private Texture2D CreateBadgeTexture()
        {
            var tex = new Texture2D(1, 1);
            var color = EditorGUIUtility.isProSkin ?
                new Color(0.25f, 0.25f, 0.25f, 0.2f) :
                new Color(0.85f, 0.85f, 0.85f, 0.4f);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private void DrawTopBorder()
        {
            var borderRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(borderRect, EditorGUIUtility.isProSkin ?
                new Color(0.1f, 0.1f, 0.1f) :
                new Color(0.5f, 0.5f, 0.5f));
        }

        /// <summary>
        /// Handles drag and drop operations for components onto the clipboard window.
        /// Accepts only Component types and adds them as new clipboard entries.
        /// </summary>
        private void HandleDragAndDrop()
        {
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                var draggedObject = DragAndDrop.objectReferences.FirstOrDefault();
                if (draggedObject is Component component)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        if (ClipboardManager.Instance != null)
                        {
                            ClipboardManager.Instance.AddClip(component, ClipType.Component);
                        }
                        Repaint();
                    }
                    Event.current.Use();
                }
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawSearchField();
            GUILayout.FlexibleSpace();
            DrawClearButton();
            DrawFavoritesToggle();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchField()
        {
            var searchRect = EditorGUILayout.GetControlRect(false, 18, EditorStyles.toolbarSearchField, GUILayout.Width(200));
            _searchQuery = EditorGUI.TextField(searchRect, _searchQuery, EditorStyles.toolbarSearchField);

            if (!string.IsNullOrEmpty(_searchQuery))
            {
                if (GUILayout.Button(GUIContent.none, GUI.skin.FindStyle("ToolbarSearchCancelButton")))
                {
                    _searchQuery = "";
                    GUI.FocusControl(null);
                }
            }
        }

        private void DrawClearButton()
        {
            var clearContent = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
            clearContent.tooltip = "Clear All Non-Favorites";

            var normalButtonColor = GUI.backgroundColor;
            var clearRect = GUILayoutUtility.GetRect(28, 24);
            var isHoveringClear = clearRect.Contains(Event.current.mousePosition);

            GUI.backgroundColor = isHoveringClear ? new Color(0.8f, 0.3f, 0.3f, 1f) : normalButtonColor;

            if (GUI.Button(clearRect, clearContent, EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("Clear Clipboard",
                    "Are you sure you want to clear all non-favorited clips?",
                    "Yes", "No"))
                {
                    ClipboardManager.Instance.ClearAllClips();
                }
            }

            GUI.backgroundColor = normalButtonColor;
        }

        private void DrawFavoritesToggle()
        {
            var favContent = EditorGUIUtility.IconContent("Favorite");
            favContent.tooltip = _showFavoritesOnly ? "Unfilter Favorites" : "Show Favorites Only";
            var favButtonStyle = new GUIStyle(EditorStyles.toolbarButton);

            Color originalGUIColor = GUI.color;
            if (_showFavoritesOnly)
            {
                GUI.color = ClipPreviewRenderer.FavoriteActiveColor;
            }

            _showFavoritesOnly = GUILayout.Toggle(_showFavoritesOnly, favContent, favButtonStyle, GUILayout.Width(28));
            GUI.color = originalGUIColor;
        }

        private void DrawClipList()
        {
            if (!_stylesInitialized || ClipboardManager.Instance == null) return;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var clips = ClipboardManager.Instance.GetClips()
                ?.Where(clip => FilterClip(clip))
                ?.ToList();

            if (clips != null && clips.Any())
            {
                foreach (var clip in clips)
                {
                    if (clip != null)
                    {
                        DrawClipEntry(clip);
                    }
                }
            }
            else
            {
                string helpBoxText = _showFavoritesOnly ? "No favorite clips available. Try adding clips to your favorites." : "No clips available. Drag and drop components here to create clips.";
                EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Filters clipboard entries based on current search, favorites, and type filter settings.
        /// Returns true if the clip should be displayed, false otherwise.
        /// </summary>
        /// <param name="clip">The clip data to evaluate</param>
        /// <returns>Boolean indicating if the clip should be displayed</returns>
        private bool FilterClip(ClipData clip)
        {
            if (_showFavoritesOnly && !clip.isFavorite)
                return false;

            if (_isTypeFiltered && _selectedComponent != null)
            {
                if (clip.DataType != _selectedComponent.GetType())
                    return false;
            }

            if (string.IsNullOrEmpty(_searchQuery))
                return true;

            return clip.sourceType.ToLower().Contains(_searchQuery.ToLower()) ||
                clip.sourcePath.ToLower().Contains(_searchQuery.ToLower());
        }

        /// <summary>
        /// Draws a single clipboard entry including its header, preview, and action buttons.
        /// </summary>
        /// <param name="clip">The clip data to display</param>
        private void DrawClipEntry(ClipData clip)
        {
            if (clip == null || clip.serializedData == null || clip.serializedData.Length == 0)
            {
                ClipboardManager.Instance.RemoveClip(clip.id, false);
                return;
            }
            EditorGUILayout.BeginVertical(_clipStyle);
            DrawClipHeader(clip);
            DrawClipPreview(clip);
            DrawClipButtons(clip);
            EditorGUILayout.EndVertical();
        }

        private void DrawClipHeader(ClipData clip)
        {
            EditorGUILayout.BeginHorizontal();

            Rect dragRect = EditorGUILayout.GetControlRect(GUILayout.Height(24));
            var dragColor = EditorGUIUtility.isProSkin ?
                new Color(0.3f, 0.3f, 0.3f, 0.3f) :
                new Color(0.8f, 0.8f, 0.8f, 0.3f);

            EditorGUI.DrawRect(dragRect, dragColor);

            var style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12
            };

            float buttonSize = 20;
            float padding = 2;
            Rect foldRect = new Rect(dragRect.xMax - (buttonSize * 2 + padding), dragRect.y + 2, buttonSize, buttonSize);
            Rect favoriteRect = new Rect(dragRect.xMax - buttonSize, dragRect.y + 2, buttonSize, buttonSize);

            var labelRect = new Rect(dragRect.x + 5, dragRect.y, dragRect.width - (buttonSize * 2 + padding + 10), dragRect.height);
            Texture componentIcon = null;
            if (clip.DataType != null)
            {
                componentIcon = EditorGUIUtility.ObjectContent(null, clip.DataType).image;

                if (componentIcon == null)
                {
                    componentIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                }
            }

            if (componentIcon != null)
            {
                var iconRect = new Rect(labelRect.x, labelRect.y + 4, 16, 16);
                GUI.DrawTexture(iconRect, componentIcon);
                labelRect.x += 20;
            }

            GUI.Label(labelRect, $"{clip.sourceType}", style);

            var foldContent = EditorGUIUtility.IconContent(clip.isExpanded ? "d_winbtn_win_max" : "d_winbtn_win_restore");
            foldContent.tooltip = clip.isExpanded ? "Collapse" : "Expand";
            if (GUI.Button(foldRect, foldContent, EditorStyles.label))
            {
                clip.isExpanded = !clip.isExpanded;
                EditorUtility.SetDirty(clip);
            }

            var favoriteContent = EditorGUIUtility.IconContent("Favorite");
            favoriteContent.tooltip = clip.isFavorite ? "Unfavorite" : "Favorite";
            var originalColor = GUI.color;
            if (clip.isFavorite)
            {
                GUI.color = ClipPreviewRenderer.FavoriteActiveColor;
            }
            if (GUI.Button(favoriteRect, favoriteContent, EditorStyles.label))
            {
                ClipboardManager.Instance.ToggleFavorite(clip.id);
            }
            GUI.color = originalColor;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawClipPreview(ClipData clip)
        {
            float previewHeight = ClipPreviewRenderer.GetPreviewHeight(clip);
            var previewRect = GUILayoutUtility.GetRect(0, previewHeight, GUILayout.ExpandWidth(true));
            ClipPreviewRenderer.DrawPreview(clip, previewRect);
        }

        private void DrawClipButtons(ClipData clip)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var normalButtonColor = GUI.backgroundColor;
            var buttonStyle = ClipPreviewRenderer.ModernButtonStyle;

            var pasteRect = GUILayoutUtility.GetRect(80, 24);
            var isHoveringPaste = pasteRect.Contains(Event.current.mousePosition);
            GUI.backgroundColor = _selectedComponent != null && isHoveringPaste ? _hoverPasteColor : normalButtonColor;

            GUI.enabled = _selectedComponent != null;
            if (GUI.Button(pasteRect, ClipPreviewRenderer.PasteContent, buttonStyle))
            {
                if (_selectedComponent != null)
                {
                    ClipboardManager.Instance.PasteClip(clip, _selectedComponent);
                }
                else if (_pasteTarget is GameObject go)
                {
                    foreach (var component in go.GetComponents<Component>())
                    {
                        ClipboardManager.Instance.PasteClip(clip, component);
                    }
                }
            }
            GUI.enabled = true;

            GUILayout.Space(5);

            var removeRect = GUILayoutUtility.GetRect(80, 24);
            var isHoveringRemove = removeRect.Contains(Event.current.mousePosition);
            GUI.backgroundColor = isHoveringRemove ? _hoverRemoveColor : normalButtonColor;

            if (GUI.Button(removeRect, ClipPreviewRenderer.RemoveContent, buttonStyle))
            {
                ClipboardManager.Instance.RemoveClip(clip.id);
            }

            GUI.backgroundColor = normalButtonColor;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an indicator showing the currently selected component and type filtering status.
        /// Includes component icon, name, and type information with a close button to clear selection.
        /// </summary>
        private void DrawSelectedComponentIndicator()
        {
            if (_selectedComponent != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                var combinedName = $"{_selectedComponent.gameObject.name}.{_selectedComponent.GetType().Name}";
                var contentWidth = EditorStyles.label.CalcSize(new GUIContent(combinedName)).x + 30;

                EditorGUILayout.BeginHorizontal(_targetIndicatorStyle, GUILayout.Width(contentWidth));

                var componentIcon = EditorGUIUtility.ObjectContent(_selectedComponent, _selectedComponent.GetType()).image;
                if (componentIcon == null)
                {
                    componentIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                }
                var iconRect = EditorGUILayout.GetControlRect(false, 16, GUILayout.Width(16));
                iconRect.y += 1;
                GUI.DrawTexture(iconRect, componentIcon, ScaleMode.ScaleToFit);

                GUILayout.Space(4);

                var labelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 0, 1, 0)
                };

                GUILayout.Label(combinedName, labelStyle);

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("×", _closeButtonStyle))
                {
                    _selectedComponent = null;
                    _isTypeFiltered = false;
                    Repaint();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Displays a filter indicator when type filtering is active, showing the selected component type
        /// and the number of filtered clips compared to total clips.
        /// </summary>
        private void DrawTypeFilterIndicator()
        {
            if (_selectedComponent != null)
            {
                _isTypeFiltered = true;
                var componentType = _selectedComponent.GetType();
                var totalClips = ClipboardManager.Instance.GetClips().Count;
                var filteredClips = ClipboardManager.Instance.GetClips()
                    .Count(c => c.DataType == componentType);

                var filterText = new System.Text.StringBuilder();
                filterText.Append(EditorGUIUtility.IconContent("FilterByType").image != null ? "   " : "");
                filterText.Append($"Filtered • {componentType.Name} ({filteredClips} of {totalClips})");

                var content = new GUIContent(filterText.ToString(), EditorGUIUtility.IconContent("FilterByType").image);
                EditorGUILayout.LabelField(content, _filterLabelStyle, GUILayout.Height(20));
            }
        }
    }
}