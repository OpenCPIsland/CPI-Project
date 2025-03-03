using System;
using UnityEngine;

namespace DevonLocalization.Core
{
	public static class LocalizationLanguage
	{
		private static string[] LANGUAGES = new string[6]
		{
			"none",
			"en_US",
			"pt_BR",
			"fr_FR",
			"es_LA",
			"oversized"
		};

		private static string[] ISO639_LANGUAGE_STRINGS = new string[6]
		{
			"en-US",
			"en-US",
			"pt-BR",
			"fr-FR",
			"es-LA",
			"en-US"
		};

		private static string[] CULTURE_LANGUAGE_STRINGS = new string[6]
		{
			"en-US",
			"en-US",
			"pt-BR",
			"fr-FR",
			"es-AR",
			"en-US"
		};

		private static string[] OneID_LANGUAGE_STRINGS = new string[6]
		{
			"en-US",
			"en-US",
			"pt-BR",
			"fr-FR",
			"es-LA",
			"en-US"
		};

		private static string[] TOKENS = new string[6];

		public static string GetLanguageString(Language langEnum)
		{
			switch (langEnum)
			{
			case Language.none:
				return LANGUAGES[0];
			case Language.en_US:
				return LANGUAGES[1];
			case Language.pt_BR:
				return LANGUAGES[2];
			case Language.fr_FR:
				return LANGUAGES[3];
			case Language.es_LA:
				return LANGUAGES[4];
			case Language.oversized:
				return LANGUAGES[5];
			default:
				return LANGUAGES[0];
			}
		}

		public static Language GetLanguageFromLanguageString(string languageString)
		{
			if (Enum.IsDefined(typeof(Language), languageString))
			{
				return (Language)Enum.Parse(typeof(Language), languageString);
			}
			return Language.none;
		}

		public static string GetCultureLanguageString(Language langEnum)
		{
			switch (langEnum)
			{
			case Language.none:
				return CULTURE_LANGUAGE_STRINGS[0];
			case Language.en_US:
				return CULTURE_LANGUAGE_STRINGS[1];
			case Language.pt_BR:
				return CULTURE_LANGUAGE_STRINGS[2];
			case Language.fr_FR:
				return CULTURE_LANGUAGE_STRINGS[3];
			case Language.es_LA:
				return CULTURE_LANGUAGE_STRINGS[4];
			case Language.oversized:
				return CULTURE_LANGUAGE_STRINGS[5];
			default:
				return CULTURE_LANGUAGE_STRINGS[1];
			}
		}

		public static string GetISOLanguageString(Language langEnum)
		{
			switch (langEnum)
			{
			case Language.none:
				return ISO639_LANGUAGE_STRINGS[0];
			case Language.en_US:
				return ISO639_LANGUAGE_STRINGS[1];
			case Language.pt_BR:
				return ISO639_LANGUAGE_STRINGS[2];
			case Language.fr_FR:
				return ISO639_LANGUAGE_STRINGS[3];
			case Language.es_LA:
				return ISO639_LANGUAGE_STRINGS[4];
			case Language.oversized:
				return ISO639_LANGUAGE_STRINGS[5];
			default:
				return ISO639_LANGUAGE_STRINGS[1];
			}
		}

		public static string GetOneIDLanguageString(Language langEnum)
		{
			switch (langEnum)
			{
			case Language.none:
				return OneID_LANGUAGE_STRINGS[0];
			case Language.en_US:
				return OneID_LANGUAGE_STRINGS[1];
			case Language.pt_BR:
				return OneID_LANGUAGE_STRINGS[2];
			case Language.fr_FR:
				return OneID_LANGUAGE_STRINGS[3];
			case Language.es_LA:
				return OneID_LANGUAGE_STRINGS[4];
			case Language.oversized:
				return OneID_LANGUAGE_STRINGS[5];
			default:
				return OneID_LANGUAGE_STRINGS[1];
			}
		}

		public static string GetCurrentLanguageString()
		{
			Language language = GetLanguage();
			return GetLanguageString(language);
		}

		public static bool TryStripLanguageStringFromEnd(string input, out string output)
		{
			for (int i = 0; i < LANGUAGES.Length; i++)
			{
				int num = input.LastIndexOf(LANGUAGES[i], StringComparison.OrdinalIgnoreCase);
				if (num == input.Length - LANGUAGES[i].Length)
				{
					output = input.Substring(0, num);
					return true;
				}
			}
			output = input;
			return false;
		}

		public static bool TryGetLanguageStringFromEnd(string input, out string language)
		{
			for (int i = 0; i < LANGUAGES.Length; i++)
			{
				int num = input.LastIndexOf(LANGUAGES[i], StringComparison.OrdinalIgnoreCase);
				if (num == input.Length - LANGUAGES[i].Length)
				{
					language = LANGUAGES[i];
					return true;
				}
			}
			language = null;
			return false;
		}

		public static Language GetLanguage()
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.Portuguese:
				return Language.pt_BR;
			case SystemLanguage.French:
				return Language.fr_FR;
			case SystemLanguage.Spanish:
				return Language.es_LA;
			case SystemLanguage.English:
				return Language.en_US;
			default:
				return Language.en_US;
			}
		}

		public static void AddLanguageToken(Language langEnum, string token)
		{
			switch (langEnum)
			{
			case Language.none:
				TOKENS[0] = token;
				break;
			case Language.en_US:
				TOKENS[1] = token;
				break;
			case Language.pt_BR:
				TOKENS[2] = token;
				break;
			case Language.fr_FR:
				TOKENS[3] = token;
				break;
			case Language.es_LA:
				TOKENS[4] = token;
				break;
			case Language.oversized:
				TOKENS[5] = token;
				break;
			}
		}

		public static string GetLanguageToken(Language langEnum)
		{
			string text = string.Empty;
			switch (langEnum)
			{
			case Language.none:
				text = TOKENS[0];
				break;
			case Language.en_US:
				text = TOKENS[1];
				break;
			case Language.pt_BR:
				text = TOKENS[2];
				break;
			case Language.fr_FR:
				text = TOKENS[3];
				break;
			case Language.es_LA:
				text = TOKENS[4];
				break;
			case Language.oversized:
				text = TOKENS[5];
				break;
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("No Language Token associated for Language: " + langEnum);
			}
			return text;
		}
	}
}
