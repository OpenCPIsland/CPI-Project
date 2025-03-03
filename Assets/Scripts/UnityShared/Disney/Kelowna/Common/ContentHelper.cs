using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class ContentHelper
	{
		public static string CDNUrl = "";

		[Conditional("FALSE")]
		public static void CheckAssetType<TExpectedBase, TAsset>()
		{
			Type typeFromHandle = typeof(TExpectedBase);
			Type typeFromHandle2 = typeof(TAsset);
			if (!typeFromHandle.IsAssignableFrom(typeFromHandle2))
			{
				throw new InvalidAssetTypeException(typeFromHandle2, typeFromHandle);
			}
		}

		public static string GetCdnUrl()
		{
			string value;
			if (!ConfigHelper.TryGetEnvironmentProperty("CDN", out value))
			{
				throw new Exception("Configurator did not contain a value for the CDN URL");
			}
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (commonGameSettings.OfflineMode && !string.IsNullOrEmpty(commonGameSettings.CDN))
			{
				value = commonGameSettings.CDN;
			}
			string text = (!string.IsNullOrEmpty(CDNUrl)) ? CDNUrl : value;
			text = text.Trim();
			if (text.EndsWith("/"))
			{
				return text;
			}
			return text + "/";
		}

		public static string GetCpipeMappingFilename()
		{
			string value;
			if (!ConfigHelper.TryGetEnvironmentProperty("CPipeMappingFilename", out value))
			{
				throw new Exception("Configurator did not contain a value for the CPipe Mapping file!");
			}
			return value;
		}

		public static string GetClientApiVersionString()
		{
			string value;
			if (!ConfigHelper.TryGetEnvironmentProperty("ClientApiVersion", out value))
			{
				return ClientInfo.GetClientVersionStr();
			}
			return value;
		}

		public static string GetPlatformString()
		{
			List<string> list = new List<string>();
			list.Add("ios");
			list.Add("android");
			list.Add("standaloneosx");
			list.Add("standalonewindows");
			List<string> list2 = list;
			string platform = ClientInfo.Instance.Platform;
			if (!list2.Contains(platform))
			{
			}
			return platform;
		}

		public static ContentManifest GetContentManifestFromAssetBundle(AssetBundle assetBundle)
		{
			string[] allAssetNames = assetBundle.GetAllAssetNames();
			ContentManifest result = null;
			for (int i = 0; i < allAssetNames.Length; i++)
			{
				string fileName = Path.GetFileName(allAssetNames[i]);
				if (string.Equals(fileName, "ContentManifest.txt", StringComparison.OrdinalIgnoreCase))
				{
					TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(allAssetNames[i]);
					result = new ContentManifest(textAsset);
					break;
				}
			}
			return result;
		}
	}
}
