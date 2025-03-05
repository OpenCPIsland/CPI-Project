using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ASoliman.Utils.ClipboardPlus
{
    /// <summary>
    /// Manages a persistent clipboard system for Unity Editor, allowing copying and pasting of component data
    /// across different objects and sessions. Supports undo/redo operations and maintains a history of clips
    /// with optional favorites.
    /// </summary>
    [InitializeOnLoad]
    public class ClipboardManager : EditorWindow
    {   
        private static ClipboardManager _instance;
        public static ClipboardManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance<ClipboardManager>();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        [SerializeField] private List<ClipData> _clips = new List<ClipData>();
        private Dictionary<string, ClipData> _clipLookup = new Dictionary<string, ClipData>();
        private readonly int _maxClips = 100;
        private string _dataPath;
        private string _clipsFilePath;

        private EditorSceneManager.SceneOpenedCallback _sceneOpenedCallback;

        private void OnEnable() => Undo.undoRedoPerformed += OnUndoRedo;
        private void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedo;
        private void OnDestroy()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= SaveClips;
            AssemblyReloadEvents.afterAssemblyReload -= LoadClips;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened -= _sceneOpenedCallback;
        }

        private void Initialize()
        {
            InitializePaths();
            Directory.CreateDirectory(_dataPath);
            LoadClips();
            RebuildLookup();

            // Subscribe to Unity editor events
            AssemblyReloadEvents.beforeAssemblyReload += SaveClips;
            AssemblyReloadEvents.afterAssemblyReload += LoadClips;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            _sceneOpenedCallback = (_, _) => LoadClips();
            EditorSceneManager.sceneOpened += _sceneOpenedCallback;
        }

        private void InitializePaths()
        {
            _dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Unity",
                "ClipSmart",
                PlayerSettings.productGUID.ToString()
            );
            
            _clipsFilePath = Path.Combine(_dataPath, "clipboard_data.json");
        }

        /// <summary>
        /// Adds a new clip to the clipboard history from the specified source object.
        /// </summary>
        /// <param name="source">The source object to create clip from</param>
        /// <param name="clipType">The type of clip to create</param>
        /// <returns>The created ClipData or null if creation fails</returns>
        public ClipData AddClip(UnityEngine.Object source, ClipType clipType)
        {
            if (source == null) return null;

            var clip = CreateInstance<ClipData>();

            try
            {
                Undo.RegisterCompleteObjectUndo(this, "Add Clipboard Item");
                clip.CaptureFromObject(source, clipType);
                
                _clips.Insert(0, clip);
                _clipLookup[clip.id] = clip;

                EnforceClipLimit();
                SaveClips();
                return clip;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating clip: {e.Message}");
                DestroyImmediate(clip);
                return null;
            }
        }

        /// <summary>
        /// Pastes the specified clip data onto the target object.
        /// </summary>
        public void PasteClip(ClipData clip, UnityEngine.Object target)
        {
            if (clip == null || target == null) return;

            Undo.RecordObject(target, "Paste Values");

            switch (clip.clipType)
            {
                case ClipType.Component when target is Component component:
                    if (clip.DataType == component.GetType())
                    {
                        PasteComponent(clip, component);
                    }
                    else
                    {
                        Debug.LogWarning($"Component type mismatch. Clip: {clip.DataType?.Name}, Target: {component.GetType().Name}");
                    }
                    break;
                default:
                    Debug.LogWarning($"Unsupported ClipType for pasting: {clip.clipType}");
                    break;
            }

            EditorUtility.SetDirty(target);
            if (!EditorApplication.isPlaying)
            {
                AssetDatabase.SaveAssets();
            }
        }

        public void RemoveClip(string id, bool registerUndo = true)
        {
            if (string.IsNullOrEmpty(id) || !_clipLookup.TryGetValue(id, out var clip))
                return;

            int index = _clips.IndexOf(clip);
            if (index == -1) return;

            if (registerUndo)
            {
                Undo.RegisterCompleteObjectUndo(this, "Remove Clipboard Item");
            }

            _clips.RemoveAt(index);
            _clipLookup.Remove(id);

            if (!registerUndo && clip != null)
            {
                DestroyImmediate(clip);
            }

            SaveClips();
        }

        public void ValidateClips()
        {
            _clips.RemoveAll(clip => clip == null ||
                                string.IsNullOrEmpty(clip?.id) ||
                                clip?.serializedData == null ||
                                clip.serializedData.Length == 0);
            
            RebuildLookup();
        }

        public List<ClipData> GetClips() => _clips;
        
        public ClipData GetRecentClipOfType(Type type) => 
            _clips.FirstOrDefault(c => c.DataType == type);

        public void ClearAllClips()
        {
            var nonFavoriteClips = _clips.Where(c => !c.isFavorite).ToList();
            foreach (var clip in nonFavoriteClips)
            {
                _clips.Remove(clip);
                _clipLookup.Remove(clip.id);
            }
            SaveClips();
        }

        public void ToggleFavorite(string id)
        {
            if (_clipLookup.TryGetValue(id, out var clip))
            {
                clip.isFavorite = !clip.isFavorite;
                SaveClips();
            }
        }

        private void PasteComponent(ClipData clip, Component target)
        {
            if (target == null) return;

            try
            {
                Undo.RecordObject(target, "Paste Component Values");

                var serializedTarget = new SerializedObject(target);
                var sourceData = System.Text.Encoding.UTF8.GetString(clip.serializedData);

                EditorJsonUtility.FromJsonOverwrite(sourceData, target);
                serializedTarget.Update();
                serializedTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error pasting component data: {e.Message}");
            }
        }

        /// <summary>
        /// Ensures the clip list doesn't exceed the maximum allowed clips by removing oldest non-favorite clips.
        /// </summary>
        private void EnforceClipLimit()
        {
            if (_clips.Count > _maxClips)
            {
                var toRemove = _clips.Where(c => !c.isFavorite)
                                .Skip(_maxClips)
                                .ToList();
                foreach (var removeClip in toRemove)
                {
                    RemoveClip(removeClip.id, true);
                }
            }
        }

        private void SaveClips()
        {
            try
            {
                var serializedData = new ClipboardData
                {
                    clips = _clips.Select(clip => new SerializedClipData
                    {
                        id = clip.id,
                        sourcePath = clip.sourcePath,
                        sourceType = clip.sourceType,
                        creationDate = clip.creationDate.ToString("o"),
                        dataTypeName = clip.DataType?.AssemblyQualifiedName,
                        clipType = clip.clipType,
                        serializedData = clip.serializedData,
                        componentPath = clip.componentPath,
                        propertyType = clip.propertyType,
                        isFavorite = clip.isFavorite,
                        isExpanded = clip.isExpanded
                    }).ToList()
                };

                string json = JsonUtility.ToJson(serializedData, true);
                File.WriteAllText(_clipsFilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving clipboard data: {e.Message}");
            }
        }

        private void LoadClips()
        {
            try
            {
                if (!File.Exists(_clipsFilePath))
                {
                    _clips.Clear();
                    return;
                }

                string json = File.ReadAllText(_clipsFilePath);
                var data = JsonUtility.FromJson<ClipboardData>(json);

                _clips.Clear();
                foreach (var serializedClip in data.clips)
                {
                    var clip = CreateInstance<ClipData>();
                    Type dataType = !string.IsNullOrEmpty(serializedClip.dataTypeName) 
                        ? Type.GetType(serializedClip.dataTypeName) 
                        : null;

                    DateTime creationDate = DateTime.TryParse(serializedClip.creationDate, out var parsed)
                        ? parsed
                        : DateTime.Now;

                    clip.Initialize(
                        serializedClip.id,
                        serializedClip.sourcePath,
                        serializedClip.sourceType,
                        creationDate,
                        dataType,
                        serializedClip.clipType,
                        serializedClip.serializedData,
                        serializedClip.componentPath,
                        serializedClip.propertyType
                    );
                    clip.isFavorite = serializedClip.isFavorite;
                    clip.isExpanded = serializedClip.isExpanded;
                    _clips.Add(clip);
                }

                RebuildLookup();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading clipboard data: {e.Message}");
                _clips.Clear();
            }
        }

        private void OnUndoRedo()
        {
            try
            {
                ValidateClips();
                ClipboardEditorWindow.ForceRepaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during undo/redo: {e.Message}");
            }
        }

        private void RebuildLookup()
        {
            _clipLookup.Clear();
            foreach (var clip in _clips.Where(c => c != null && !string.IsNullOrEmpty(c.id)))
            {
                if (!_clipLookup.ContainsKey(clip.id))
                {
                    _clipLookup[clip.id] = clip;
                }
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    SaveClips();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    LoadClips();
                    ClipboardEditorWindow.ForceRepaint();
                    break;
            }
        }

        [Serializable]
        private class ClipboardData
        {
            public List<SerializedClipData> clips = new List<SerializedClipData>();
        }

        [Serializable]
        private class SerializedClipData
        {
            public string id;
            public string sourcePath;
            public string sourceType;
            public string creationDate;
            public string dataTypeName;
            public ClipType clipType;
            public byte[] serializedData;
            public string componentPath;
            public SerializedPropertyType propertyType;
            public bool isFavorite;
            public bool isExpanded;
        }
    }
}