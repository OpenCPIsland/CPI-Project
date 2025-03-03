using DisneyMobile.CoreUnitySystems.Utility;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class TextManager : MonoBehaviour, IConfigurable
	{
		public ArrayList mLanguageInfo;

		private Dictionary<Language, LanguageInfo> mLanguageDict = new Dictionary<Language, LanguageInfo>();

		private Dictionary<Language, Dictionary<string, string>> mDictionaries = new Dictionary<Language, Dictionary<string, string>>();

		private Language currentLang;

		private static TextManager mInstance;

		public static TextManager Instance
		{
			get
			{
				return mInstance;
			}
		}

		public static event Action<Language> LanguageChangeEvent;

		public void Awake()
		{
			mInstance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void Initialize()
		{
			Logger.LogInfo(this, "TextManager Initialize called");
			CreateLanguageDict();
			currentLang = Language.INVALID;
			Language language = GetDeviceLanguage();
			string @string = PlayerPrefs.GetString("User_Language", "");
			if (@string != "")
			{
				language = Utilities.ParseForEnum(@string, language);
			}
			SetLanguage(language);
		}

		public void Configure(IDictionary<string, object> dictionary)
		{
			AutoConfigurable.AutoConfigureObject(this, dictionary);
		}

		public void Reconfigure(IDictionary<string, object> dictionary)
		{
			Configure(dictionary);
		}

		private void CreateLanguageDict()
		{
			foreach (object item in mLanguageInfo)
			{
				string text = "";
				text = ((!(item is string)) ? JsonMapper.ToJson(item) : ((string)item));
				LanguageInfo languageInfo = JsonMapper.ToObject<LanguageInfo>(text);
				Language language = VerifyLanguage(languageInfo);
				if (language != Language.INVALID)
				{
					if (!mLanguageDict.ContainsKey(language))
					{
						mLanguageDict.Add(language, languageInfo);
						Logger.LogInfo(this, "mLanguageDict.Add: " + languageInfo.mName + " => " + languageInfo.mFileName);
					}
					else
					{
						Logger.LogFatal(this, "Duplicate Language Info = " + language);
					}
				}
				else
				{
					Logger.LogFatal(this, "Invalid Language name = " + languageInfo.mName);
				}
			}
		}

		private Language VerifyLanguage(LanguageInfo langInfo)
		{
			return Utilities.ParseForEnum(langInfo.mName, Language.INVALID);
		}

		private void LoadLanguageFileToDict(Language lang)
		{
			if (mDictionaries.ContainsKey(lang))
			{
				currentLang = lang;
				return;
			}
			currentLang = Language.ENGLISH_NTSC;
			LanguageInfo value;
			if (mLanguageDict.TryGetValue(lang, out value))
			{
				TextAsset textAsset = Resources.Load(value.mFileName, typeof(TextAsset)) as TextAsset;
				if (textAsset != null)
				{
					ByteReader byteReader = new ByteReader(textAsset);
					mDictionaries.Add(lang, byteReader.ReadDictionary());
					currentLang = lang;
					Logger.LogInfo(this, value.mName + " language file loaded.");
				}
				else
				{
					Logger.LogWarning(this, "File " + value.mFileName + " does not exist. Please check config file. Load English by default.");
					if (lang != 0)
					{
						LoadLanguageFileToDict(Language.ENGLISH_NTSC);
					}
				}
			}
			else
			{
				Logger.LogWarning(this, "Language " + lang.ToString() + " not found in dictionary. Please check config file. Load English by default.");
				if (lang != 0)
				{
					LoadLanguageFileToDict(Language.ENGLISH_NTSC);
				}
			}
		}

		private Language GetChineseLanguage()
		{
			Language result = Language.CHINESE_TRADITIONAL;
			string a = EnvironmentManager.Locale.ToLower();
			if (a == "zh_cn" || a == "zh-hans" || a == "zh-sg")
			{
				result = Language.CHINESE_SIMPLIFIED;
			}
			return result;
		}

		private void ClearAllDictionaries()
		{
			mDictionaries.Clear();
		}

		public string GetString(string id)
		{
			string value = "Cannot find " + currentLang.ToString() + " in dictionary";
			Dictionary<string, string> value2;
			if (mDictionaries.TryGetValue(currentLang, out value2) && !value2.TryGetValue(id, out value))
			{
				value = "NotFound_" + id;
				Logger.LogWarning(this, "Cannot find string id " + id);
			}
			return value;
		}

		public void SetLanguage(Language lang)
		{
			if (currentLang != lang)
			{
				Language language = currentLang;
				PlayerPrefs.SetString("User_Language", lang.ToString());
				LoadLanguageFileToDict(lang);
				if (TextManager.LanguageChangeEvent != null && language != currentLang)
				{
					TextManager.LanguageChangeEvent(currentLang);
				}
			}
		}

		public Language GetCurrentLanguage()
		{
			return currentLang;
		}

		public Language GetDeviceLanguage()
		{
			Language result = Language.ENGLISH_NTSC;
			switch (Application.systemLanguage)
			{
			case SystemLanguage.English:
				result = Language.ENGLISH_NTSC;
				break;
			case SystemLanguage.French:
				result = Language.FRENCH_NTSC;
				break;
			case SystemLanguage.Italian:
				result = Language.ITALIAN;
				break;
			case SystemLanguage.German:
				result = Language.GERMAN;
				break;
			case SystemLanguage.Spanish:
				result = Language.SPANISH_NTSC;
				break;
			case SystemLanguage.Japanese:
				result = Language.JAPANESE;
				break;
			case SystemLanguage.Korean:
				result = Language.KOREAN;
				break;
			case SystemLanguage.Chinese:
				result = GetChineseLanguage();
				break;
			case SystemLanguage.Russian:
				result = Language.RUSSIAN;
				break;
			case SystemLanguage.Dutch:
				result = Language.DUTCH;
				break;
			case SystemLanguage.Czech:
				result = Language.CZECH;
				break;
			case SystemLanguage.Polish:
				result = Language.POLISH;
				break;
			case SystemLanguage.Portuguese:
				result = Language.PORTUGUESE_BRAZILIAN;
				break;
			}
			return result;
		}

		public void TestTextManager()
		{
			Logger.LogInfo(this, "Current language: " + currentLang);
			Logger.LogInfo(this, "Test: " + GetString("Btn_No"));
			Logger.LogInfo(this, "Test: " + GetString("Btn_Test"));
			Logger.LogInfo(this, "Test: " + GetString("Lbl_FastTravel"));
			SetLanguage(Language.CHINESE_SIMPLIFIED);
			Logger.LogInfo(this, "Current language: " + currentLang);
			Logger.LogInfo(this, "Test: " + GetString("Btn_No"));
			Logger.LogInfo(this, "Test: " + GetString("Btn_Test"));
			Logger.LogInfo(this, "Number of languages in dictionary: " + mDictionaries.Count);
		}

		public int CountDict()
		{
			return mDictionaries.Count;
		}
	}
}
