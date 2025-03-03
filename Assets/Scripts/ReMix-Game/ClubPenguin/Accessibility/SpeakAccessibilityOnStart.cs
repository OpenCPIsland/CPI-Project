using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Accessibility
{
	public class SpeakAccessibilityOnStart : MonoBehaviour
	{
		public string AccessibilitySpeechToken;

		public Text[] TextReferenceObjects;

		private void Start()
		{
			speakAcessibilityToken();
		}

		private void speakAcessibilityToken()
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel != NativeAccessibilityLevel.VOICE || string.IsNullOrEmpty(AccessibilitySpeechToken))
			{
				return;
			}
			string text = Service.Get<Localizer>().GetTokenTranslation(AccessibilitySpeechToken);
			if (TextReferenceObjects != null && TextReferenceObjects.Length > 0)
			{
				for (int i = 0; i < TextReferenceObjects.Length; i++)
				{
					text = text + " " + TextReferenceObjects[i].text;
				}
			}
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(text);
		}
	}
}
