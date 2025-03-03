using DevonLocalization;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	public class TextStyle : MonoBehaviour
	{
		public TextStyleOptions Style;

		private LocalizedText localizedText;

		private Text text;

		private void OnEnable()
		{
			text = GetComponent<Text>();
			applyStyle(text.text);
			localizedText = GetComponent<LocalizedText>();
			if (localizedText != null && !localizedText.doNotLocalize)
			{
				localizedText.OnUpdateToken += applyStyle;
			}
		}

		private void OnDisable()
		{
			if (localizedText != null && !localizedText.doNotLocalize)
			{
				localizedText.OnUpdateToken -= applyStyle;
			}
		}

		protected virtual void applyStyle(string newText)
		{
			switch (Style)
			{
			case TextStyleOptions.LowerCase:
				newText = newText.ToLower();
				break;
			case TextStyleOptions.UpperCase:
				newText = newText.ToUpper();
				break;
			case TextStyleOptions.TitleCase:
			{
				TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
				newText = textInfo.ToTitleCase(newText);
				break;
			}
			}
			text.text = newText;
		}
	}
}
