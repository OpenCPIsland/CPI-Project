using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework.Utility
{
	public static class ConfigurationHelper
	{
		public static IDictionary<string, object> GetSKUDictionaryFromSystemDictionary(IDictionary<string, object> valuesDictionary, string targetSkuName)
		{
			IDictionary<string, object> result = null;
			ArrayList arrayList = valuesDictionary["SKUs"] as ArrayList;
			if (arrayList != null)
			{
				for (int i = 0; i < arrayList.Count; i++)
				{
					Dictionary<string, object> dictionary = arrayList[i].AsDic() as Dictionary<string, object>;
					foreach (KeyValuePair<string, object> item in dictionary)
					{
						string key = item.Key;
						if (string.Equals(key, targetSkuName, StringComparison.OrdinalIgnoreCase))
						{
							result = dictionary[key].AsDic();
							break;
						}
					}
				}
			}
			return result;
		}

		public static RuntimePlatform? GetRuntimePlatformFromString(string platformName)
		{
			switch (platformName)
			{
			case "iOS":
				return RuntimePlatform.IPhonePlayer;
			case "Android":
				return RuntimePlatform.Android;
			default:
				return null;
			}
		}
	}
}
