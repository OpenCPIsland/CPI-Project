using UnityEngine;
using UnityEditor;
using System;

namespace ASoliman.Utils.ClipboardPlus
{
    /// <summary>
    /// Represents a data container for storing and managing clipboard data in the Unity Editor.
    /// This ScriptableObject handles serialization and deserialization of various Unity object types
    /// for clipboard operations.
    /// </summary>
    [Serializable]
    public class ClipData : ScriptableObject
    {
        public string id = Guid.NewGuid().ToString();
        public string sourcePath;
        public string sourceType;
        public DateTime creationDate;
        public bool isFavorite;
        public ClipType clipType;
        public byte[] serializedData;
        public string componentPath;
        public SerializedPropertyType propertyType;
        public bool isExpanded = false;

        public Type DataType { get; set; }

        /// <summary>
        /// Initializes the ClipData instance with the provided values.
        /// </summary>
        public void Initialize(string id, string sourcePath, string sourceType, DateTime creationDate, Type dataType, 
            ClipType clipType, byte[] serializedData, string componentPath, SerializedPropertyType propertyType)
        {
            this.id = id;
            this.sourcePath = sourcePath;
            this.sourceType = sourceType;
            this.creationDate = creationDate;
            this.DataType = dataType;
            this.clipType = clipType;
            this.serializedData = serializedData;
            this.componentPath = componentPath;
            this.propertyType = propertyType;
        }

        /// <summary>
        /// Captures and serializes data from a Unity Object based on the specified ClipType.
        /// Currently supports Component type captures.
        /// </summary>
        /// <param name="source">The Unity Object to capture data from</param>
        /// <param name="type">The type of clip to create</param>
        public void CaptureFromObject(UnityEngine.Object source, ClipType type)
        {
            if (source == null) return;

            Undo.RegisterCompleteObjectUndo(this, "Capture Object Data");
            clipType = type;
            DataType = source.GetType();
            creationDate = DateTime.Now;

            try
            {
                switch (type)
                {
                    case ClipType.Component:
                        var component = source as Component;
                        if (component != null)
                        {
                            sourcePath = component.gameObject.name;
                            sourceType = component.GetType().Name;
                            CaptureComponent(component);
                        }
                        break;
                }

                EditorUtility.SetDirty(this);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error capturing object data: {e.Message}");
                serializedData = null;
            }
        }

        private void CaptureComponent(Component component)
        {
            if (component == null) return;
            serializedData = System.Text.Encoding.UTF8.GetBytes(EditorJsonUtility.ToJson(component));
        }
    }
}