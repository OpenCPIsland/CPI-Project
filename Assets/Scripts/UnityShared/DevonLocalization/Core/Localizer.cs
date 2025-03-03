using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace DevonLocalization.Core
{
	public class Localizer : IAccessibilityLocalization
	{
		public delegate void TokensUpdatedDelegate();

		public static string DEFAULT_TOKEN_LOCATION = "Generated/Resources/Translations/";

		public static string PLATFORM = "platform";

		public static string TYPE = "type";

		public static string SUBTYPE = "subtype";

		public static string TOKENS = "tokens";

		public static string RELEASE = "release";

		private static Localizer _Instance;

		public TokensUpdatedDelegate TokensUpdated;

		public Language Language = Language.en_US;

		public string LanguageString = "en_US";

		public string LanguageStringOneID = "en-US";

		public Dictionary<string, string> tokens = new Dictionary<string, string>();

		private JsonService jsonService;

		public TokenType TokenTypes;

		public static Localizer Instance
		{
			get
			{
				if (_Instance == null)
				{
					if (Service.IsSet<Localizer>())
					{
						_Instance = Service.Get<Localizer>();
						Debug.Log("Using Service version of Localizer");
					}
					else
					{
						Debug.Log("Using Singleton version of Localizer");
						_Instance = new Localizer();
					}
				}
				return _Instance;
			}
		}

		public Localizer()
		{
			if (jsonService == null)
			{
				if (Service.IsSet<JsonService>())
				{
					jsonService = Service.Get<JsonService>();
				}
				else
				{
					jsonService = new LitJsonService();
				}
			}
			TokenTypes = new TokenType();
		}

		public void ChangeLanguage(Language language)
        {
			if (language != Language)
			{
				ResetTokens();
				Language = language;
				LanguageString = LocalizationLanguage.GetLanguageString(language);
				LanguageStringOneID = LocalizationLanguage.GetOneIDLanguageString(language);
				Thread.CurrentThread.CurrentCulture = new CultureInfo(LocalizationLanguage.GetCultureLanguageString(language));
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(LocalizationLanguage.GetCultureLanguageString(language));
				ReLoadTokens();
			}
		}

		public void ResetTokens()
		{
			tokens = new Dictionary<string, string>();
		}

		public string GetTokenTranslation(string token)
		{
			if (tokens.Count == 0)
			{
				return "";
			}
			if (!string.IsNullOrEmpty(token))
			{
				token = token.Trim();
				string value;
				if (tokens.Count > 0 && tokens.TryGetValue(token, out value))
				{
					return value;
				}
			}
			if (string.IsNullOrEmpty(token))
			{
				return token;
			}
			return token;
		}

		public string GetTokenTranslationFormatted(string formatToken, params string[] argTokens)
		{
			string tokenTranslation = GetTokenTranslation(formatToken);
			string[] array = new string[argTokens.Length];
			for (int i = 0; i < argTokens.Length; i++)
			{
				array[i] = GetTokenTranslation(argTokens[i]);
			}
			return string.Format(tokenTranslation, array);
		}

		public string GetTokenForCurrentEnvironment(string baseToken)
		{
			return baseToken;
		}

		public void LoadTokensFromLocalJSON(ILocalizedTokenFilePath path, Action<bool> responseHandler = null)
		{
			string filePathFromPath = getFilePathFromPath(path);
			TextAsset textAsset = Resources.Load<TextAsset>(filePathFromPath);
			if (textAsset == null)
			{
				string message = string.Format("Could not load JSON from Resources filePath {0}", filePathFromPath);
				Log.LogError(this, message);
				if (responseHandler != null)
				{
					responseHandler(false);
				}
			}
			else
			{
				string text = textAsset.text;
				UpdateTokensFromJSONText(text, responseHandler);
				try
				{
					if (Service.IsSet<Content>())
					{
						LoadTokensFromContentSystem(path, responseHandler);
					}
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
				}
			}
		}

		public void LoadTokensFromContentSystem(ILocalizedTokenFilePath path, Action<bool> responseHandler = null)
		{
			string filePathFromPath = getFilePathFromPath(path);
			CoroutineRunner.Start(loadTranslationsAsync(filePathFromPath, responseHandler), this, "loadTranslationsAsync");
		}

		private IEnumerator loadTranslationsAsync(string filePathKey, Action<bool> responseHandler = null)
		{
			AssetRequest<TextAsset> assetRequest2 = null;
			while (assetRequest2 == null)
			{
				try
				{
					assetRequest2 = Content.LoadAsync<TextAsset>(filePathKey);
				}
				catch
				{
				}
				yield return null;
			}
			try
			{
				assetRequest2 = Content.LoadAsync<TextAsset>(filePathKey);
			}
			catch (Exception)
			{
				assetRequest2 = null;
				if (responseHandler != null)
				{
					responseHandler(false);
				}
			}
			yield return assetRequest2;
			if (assetRequest2 == null)
			{
				yield break;
			}
			TextAsset asset = assetRequest2.Asset;
			if (asset == null)
			{
				Log.LogErrorFormatted(this, "Could not load JSON from Content using Key {0}", filePathKey);
				if (responseHandler != null)
				{
					responseHandler(false);
				}
			}
			else
			{
				string text = asset.text;
				UpdateTokensFromJSONText(text, responseHandler);
			}
		}

		private string getFilePathFromPath(ILocalizedTokenFilePath path)
		{
			return (Language == Language.oversized) ? path.GetResourceFilePathForLanguage(LocalizationLanguage.GetLanguageString(Language.en_US)) : path.GetResourceFilePathForLanguage(LanguageString);
		}

		public void UpdateTokensFromJSONText(string jsonText, Action<bool> responseHandler)
		{
			Dictionary<string, string> dictionary = jsonService.Deserialize<Dictionary<string, string>>(jsonText);
			if (dictionary != null && dictionary.Count != 0)
			{
				bool obj = false;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					string text = item.Value;
					if (Language == Language.oversized)
					{
						int num = (int)((double)text.Length * 0.4) - 4;
						string text2 = "";
						if (num > 0)
						{
							int num2 = (int)Math.Floor((double)num / 6.0);
							int num3 = num - num2 * 6;
							for (int i = 0; i < num2; i++)
							{
								text2 = text2 + " " + Guid.NewGuid().ToString("N").Substring(0, 5);
							}
							if (num3 > 0)
							{
								text2 = text2 + " " + Guid.NewGuid().ToString("N").Substring(0, num3);
							}
						}
						text = "@@" + text + text2 + "@@";
					}
					if (tokens.ContainsKey(item.Key))
					{
						obj = true;
						tokens[item.Key] = text;
					}
					else
					{
						obj = true;
						tokens.Add(item.Key, text);
					}
				}
				if (responseHandler != null)
				{
					responseHandler(obj);
				}
				if (TokensUpdated != null)
				{
					TokensUpdated();
				}
			}
		}

		public void ReLoadTokens()
		{
			ILocalizedTokenFilePath path = new AppTokensFilePath(DEFAULT_TOKEN_LOCATION, Platform.global);
			ResetTokens();
			LoadTokensFromLocalJSON(path);
		}

		string IAccessibilityLocalization.GetString(string aToken)
		{
			return GetTokenTranslation(aToken);
		}
	}
}
