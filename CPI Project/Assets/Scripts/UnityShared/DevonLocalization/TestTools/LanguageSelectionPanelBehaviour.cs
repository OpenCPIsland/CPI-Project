using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace DevonLocalization.TestTools
{
	public class LanguageSelectionPanelBehaviour : MonoBehaviour
	{
		public LanguagesPanelBehaviour LanguagePanel;

		public string PathToTokens = "Assets/Framework/DevonLocalization/Resources/Translations";

		public string ModuleId = "login";

		public Platform platform = Platform.global;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			LanguagesPanelBehaviour languagePanel = LanguagePanel;
			languagePanel.OnLanguageChanged = (LanguagesPanelBehaviour.OnLanguageChangedDelegate)Delegate.Combine(languagePanel.OnLanguageChanged, new LanguagesPanelBehaviour.OnLanguageChangedDelegate(ChangeLanguage));
		}

		public void TogglePanel()
		{
			LanguagePanel.gameObject.SetActive(!LanguagePanel.gameObject.activeSelf);
		}

		public void ChangeLanguage(Language language)
		{
			Service.Get<Localizer>().ResetTokens();
			Service.Get<Localizer>().Language = language;
			Service.Get<Localizer>().LanguageString = LocalizationLanguage.GetLanguageString(language);
			ILocalizedTokenFilePath path = string.IsNullOrEmpty(ModuleId) ? ((ILocalizedTokenFilePath)new AppTokensFilePath(PathToTokens)) : ((ILocalizedTokenFilePath)new ModuleTokensFilePath(PathToTokens, ModuleId, platform));
			Service.Get<Localizer>().LoadTokensFromLocalJSON(path);
		}
	}
}
