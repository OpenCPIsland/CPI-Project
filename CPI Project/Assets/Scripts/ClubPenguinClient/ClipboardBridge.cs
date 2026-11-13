using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Net.Offline
{
    public class ClipboardBridge : MonoBehaviour
    {
        public static ClipboardBridge Instance { get; private set; }

        private Action<string> _pasteCallback;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void CopyToClipboard(string text);

        [DllImport("__Internal")]
        private static extern void PasteFromClipboard(string gameObject, string method);
#endif

        public void Copy(string text)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            CopyToClipboard(text);
#else
            GUIUtility.systemCopyBuffer = text;
#endif
        }

        public void RequestPaste(Action<string> callback)
        {
            _pasteCallback = callback;

#if UNITY_WEBGL && !UNITY_EDITOR
            PasteFromClipboard(
                gameObject.name,
                nameof(OnClipboardText)
            );
#else
            OnClipboardText(GUIUtility.systemCopyBuffer);
#endif
        }

        public void OnClipboardText(string text)
        {
            _pasteCallback?.Invoke(text);
            _pasteCallback = null;
        }
    }
}
