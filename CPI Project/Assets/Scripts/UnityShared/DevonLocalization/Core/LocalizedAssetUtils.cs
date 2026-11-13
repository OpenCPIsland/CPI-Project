using System;

namespace DevonLocalization.Core
{
	public static class LocalizedAssetUtils
	{
		public static bool IsLocalizedKey(string key)
		{
			string output;
			if (LocalizationLanguage.TryStripLanguageStringFromEnd(key, out output))
			{
				return output.EndsWith("_");
			}
			return false;
		}

		public static string GetGenericKey(string key, string assetPath)
		{
			string output;
			if (LocalizationLanguage.TryStripLanguageStringFromEnd(key, out output))
			{
				if (output.EndsWith("_"))
				{
					return output.TrimEnd('_');
				}
				throw new InvalidOperationException(string.Format("Key {0} does not have an underscore before the language, a generic key cannot be created for file {1}", key, assetPath));
			}
			throw new InvalidOperationException(string.Format("Key {0} does not have a valid language string at the end, a generic key cannot be created for file {1}", key, assetPath));
		}
	}
}
