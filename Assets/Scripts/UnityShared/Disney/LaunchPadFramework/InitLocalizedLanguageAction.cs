using DevonLocalization;
using DevonLocalization.Core;
using System.Collections;

namespace Disney.LaunchPadFramework
{
	public class InitLocalizedLanguageAction : InitActionComponent
	{
		public LanguageTokenConfig Config;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			if (Config != null)
			{
				int num = Config.LanguageTokens.Length;
				for (int i = 0; i < num; i++)
				{
					LanguageTokenConfig.LanguageToken languageToken = Config.LanguageTokens[i];
					LocalizationLanguage.AddLanguageToken(languageToken.Language, languageToken.Token);
				}
			}
			yield break;
		}
	}
}
