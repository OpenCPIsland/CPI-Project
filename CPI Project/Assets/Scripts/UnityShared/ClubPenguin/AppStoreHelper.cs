using DevonLocalization.Core;
using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public static class AppStoreHelper
	{
		public static string GetAppURL(Environment devEnvironment = Environment.PRODUCTION)
		{
			string text = "";
			text = "Windows";
			IDictionary<string, object> skuDictionary = getSkuDictionary(text);
			string value;
			if (tryGetValue(skuDictionary, text, "DownloadURLLocToken", out value))
			{
				Localizer localizer = Service.Get<Localizer>();
				return localizer.GetTokenTranslation(localizer.GetTokenForCurrentEnvironment(value));
			}
			return null;
		}

		public static string GetManageSubscriptionsURL()
		{
			string platformKey = "";
			string key = "";
			IDictionary<string, object> skuDictionary = getSkuDictionary(platformKey);
			string value;
			string value2;
			if (tryGetValue(skuDictionary, platformKey, "ManageSubscriptionsURIFormat", out value) && tryGetValue(skuDictionary, platformKey, key, out value2))
			{
				return string.Format(value, value2);
			}
			return null;
		}

		private static IDictionary<string, object> getSkuDictionary(string platformKey)
		{
			Configurator configurator = Service.Get<Configurator>();
			if (!configurator.IsSystemEnabled("SKU"))
			{
				Log.LogError(typeof(AppStoreHelper), "AppStoreHelper: ApplicationConfig.txt did not contain SKU system.");
				return null;
			}
			IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("SKU");
			IDictionary<string, object> dictionary = dictionaryForSystem["values"] as IDictionary<string, object>;
			return ConfigurationHelper.GetSKUDictionaryFromSystemDictionary(dictionary[platformKey].AsDic(), platformKey);
		}

		private static bool tryGetValue(IDictionary<string, object> skuDictionary, string platformKey, string key, out string value)
		{
			object value2;
			if (!skuDictionary.TryGetValue(key, out value2))
			{
				Log.LogErrorFormatted(typeof(AppStoreHelper), "{0} SKU dictionary did not contain the {1} value", platformKey, key);
				value = null;
				return false;
			}
			value = value2.ToString();
			return true;
		}
	}
}
