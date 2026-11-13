using UnityEngine;

namespace DevonLocalization
{
	[RequireComponent(typeof(LocalizedText))]
	public class PlatformSpecificText : MonoBehaviour
	{
		public string IOSToken;

		public string AndroidToken;

		public string StandaloneToken;

		public bool AllowEmpty = false;

		private LocalizedText localizedText;

		private void Start()
		{
			string text = null;
			text = StandaloneToken;
			if (AllowEmpty || !string.IsNullOrEmpty(text))
			{
				localizedText = GetComponent<LocalizedText>();
				localizedText.token = text;
				localizedText.UpdateToken();
			}
		}
	}
}
