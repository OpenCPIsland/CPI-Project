using UnityEngine;

namespace DevonLocalization
{
    public class PlatformSpecificTextTMP : MonoBehaviour
    {
        public string IOSToken;
        public string AndroidToken;
        public string StandaloneToken;
        public bool AllowEmpty = false;

        private LocalizedText localizedText;

        private void Start()
        {
            string text = null;

#if UNITY_IOS
            text = IOSToken;
#elif UNITY_ANDROID
            text = AndroidToken;
#else
            text = StandaloneToken;
#endif

            localizedText = GetComponent<LocalizedText>();

            if (localizedText != null && (AllowEmpty || !string.IsNullOrEmpty(text)))
            {
                localizedText.token = text;
                localizedText.UpdateToken();
            }
        }
    }
}