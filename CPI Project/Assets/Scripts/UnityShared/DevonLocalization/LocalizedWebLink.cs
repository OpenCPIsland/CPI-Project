using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevonLocalization
{
	public class LocalizedWebLink : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public delegate void WebLinkClickedDelegate(string url);

		public bool OpenURLOnClick = true;

		public WebLinkClickedDelegate WebLinkClicked;

		public LocalizedText localizedText;

		protected Button linkButton;

		protected string URL
		{
			get;
			private set;
		}

		private void Awake()
		{
			if (localizedText == null)
			{
				localizedText = GetComponent<LocalizedText>();
			}
			if (localizedText == null)
			{
				throw new MissingReferenceException("LocalizedWebLink requires a reference to a LocalizedText component");
			}
			localizedText.doNotLocalize = true;
			localizedText.OnUpdateToken += UpdateToken;
			linkButton = GetComponent<Button>();
		}

		private void OnDestroy()
		{
			localizedText.OnUpdateToken -= UpdateToken;
		}

		private void UpdateToken(string translation)
		{
			string input = localizedText.StripQuoteSlashes(GetHTMLLinkText(localizedText.TranslatedText));
			input = AbstractLocalizedText.StripVisibleNewLineCharacters(input);
			localizedText.GetComponent<Text>().text = input;
		}

		protected virtual string GetHTMLLinkText(string input)
		{
			string pattern = "(.*href\\s?=\\s?\\\\\\\")(.*)(\\\\\\\"\\s?>)(.*)(<\\s?\\/\\s?a\\s?>)";
			MatchCollection matchCollection = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
			if (matchCollection.Count != 1)
			{
				return "No HTML Link in translation";
			}
			Match match = matchCollection[0];
			if (match.Groups.Count != 6)
			{
				return "No HTML Link in translation";
			}
			URL = match.Groups[2].Value;
			return match.Groups[4].Value;
		}

		public virtual void OnPointerClick(PointerEventData data)
		{
			if (!string.IsNullOrEmpty(URL) && (linkButton == null || linkButton.IsInteractable()))
			{
				OnWebLinkClicked();
			}
		}

		protected virtual void OnWebLinkClicked()
		{
			if (OpenURLOnClick)
			{
				OpenURL();
			}
			if (WebLinkClicked != null)
			{
				WebLinkClicked(URL);
			}
		}

		protected virtual void OpenURL()
		{
			Application.OpenURL(URL);
		}
	}
}
