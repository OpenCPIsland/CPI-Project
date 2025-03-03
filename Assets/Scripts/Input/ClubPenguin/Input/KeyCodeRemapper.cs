using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class KeyCodeRemapper
	{
		private Dictionary<KeyCode, KeyCode> keyCodeRemaps = new Dictionary<KeyCode, KeyCode>();

		public IEnumerator Initialize()
		{
			AppTokensFilePath remapDataFilePath = new AppTokensFilePath("Resources/KeyCodeRemaps/", Platform.global, string.Empty);
			string filePathKey = getFilePathFromPath(remapDataFilePath);
			AssetRequest<KeyCodeRemapData> assetRequest;
			try
			{
				assetRequest = Content.LoadAsync<KeyCodeRemapData>(filePathKey);
			}
			catch (Exception)
			{
				assetRequest = null;
			}
			yield return assetRequest;
			if (assetRequest != null)
			{
				int num = assetRequest.Asset.KeyCodeRemaps.Length;
				for (int i = 0; i < num; i++)
				{
					KeyCodeRemapData.KeyCodeRemap keyCodeRemap = assetRequest.Asset.KeyCodeRemaps[i];
					keyCodeRemaps[keyCodeRemap.KeyCode] = keyCodeRemap.RemappedKeyCode;
				}
			}
		}

		private string getFilePathFromPath(ILocalizedTokenFilePath path)
		{
			Language language = Service.Get<Localizer>().Language;
			language = ((language == Language.oversized) ? Language.en_US : language);
			return path.GetResourceFilePathForLanguage(LocalizationLanguage.GetLanguageString(language));
		}

		public KeyCode GetKeyCode(KeyCode keyCode)
		{
			return keyCodeRemaps.ContainsKey(keyCode) ? keyCodeRemaps[keyCode] : keyCode;
		}
	}
}
