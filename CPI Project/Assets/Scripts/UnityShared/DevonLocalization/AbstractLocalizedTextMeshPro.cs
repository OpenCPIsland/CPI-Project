using System;
using TMPro;
using UnityEngine;
using DevonLocalization.Core;
using Disney.MobileNetwork;

namespace DevonLocalization
{
	public abstract class AbstractLocalizedTextMeshPro : MonoBehaviour
	{
		[LocalizationToken]
		public string token = "";

		public bool doNotLocalize = false;
		public bool stripVisibleNewline = true;
		public Platform platform;

		[HideInInspector]
		public string tokenType;

		[HideInInspector]
		public string tokenSubType;

		private Localizer localizerInstance;

		public string TranslatedText { get; private set; }

		public abstract string TextFieldText { get; }

		public event Action<string> OnUpdateToken;

		protected abstract void setText(string text);
		protected abstract void awake();

		private void Awake()
		{
			if (Service.IsSet<Localizer>())
			{
				localizerInstance = Service.Get<Localizer>();
			}
			else
			{
				localizerInstance = Localizer.Instance;
			}
			awake();
		}

		private void OnEnable()
		{
			Localizer localizer = localizerInstance;
			localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Combine(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(UpdateToken));
			UpdateToken();
		}

		private void OnDisable()
		{
			Localizer localizer = localizerInstance;
			localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Remove(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(UpdateToken));
		}

		public string StripQuoteSlashes(string input)
		{
			return input.Replace("\\\"", "\"");
		}

		public static string StripVisibleNewLineCharacters(string input)
		{
			bool flag = false;
			if (input.Contains("\\r"))
			{
				input = input.Replace("\\r", " ");
				flag = true;
			}
			if (input.Contains("\\n"))
			{
				input = input.Replace("\\n", flag ? string.Empty : " ");
			}
			return input;
		}

		public void UpdateToken()
		{
			TranslatedText = localizerInstance.GetTokenTranslation(token);
			OnUpdateToken?.Invoke(TranslatedText);

			if (doNotLocalize)
			{
				return;
			}

			setText(StripQuoteSlashes(TranslatedText));

			if (stripVisibleNewline)
			{
				TranslatedText = StripVisibleNewLineCharacters(TranslatedText);
			}
		}
	}
}
