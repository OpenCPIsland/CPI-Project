using UnityEditor;
using UnityEngine;

namespace Kamgam.LEF
{
    [System.Serializable]
    public class ScriptFile : ISerializationCallbackReceiver
    {
        /// <summary>
        /// File GUID.
        /// </summary>
        public string GUID;

        /// <summary>
        /// This is the asset path relative to the project root (starts with "Assets/...)".
        /// </summary>
        public string Path;

        private bool miniThumbnailLoaded;
        protected Texture miniThumbnail;
        public Texture MiniThumbnail
        {
            get
            {
                if (!miniThumbnailLoaded)
                    loadThumbnail();
                return miniThumbnail;
            }

            set
            {
                miniThumbnail = value;
            }
        }

        public ScriptFile(string assetGUID, string path)
        {
            GUID = assetGUID;
            Path = path;

            loadThumbnail();
        }

        public ScriptFile(ScriptFile objectToCopy)
        {
            GUID = objectToCopy.GUID;
            Path = objectToCopy.Path;

            loadThumbnail();
        }

        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize()
        {
            miniThumbnailLoaded = false;
        }

        protected void loadThumbnail()
        {
            MiniThumbnail = EditorGUIUtility.FindTexture("cs Script Icon");

            // for assets other than scripts (which we don't have here)
            /*
            var asset = AssetDatabase.LoadAssetAtPath<Object>(Path);
            if (asset != null)
                MiniThumbnail = AssetPreview.GetMiniThumbnail(asset);
            */
        }
    }
}