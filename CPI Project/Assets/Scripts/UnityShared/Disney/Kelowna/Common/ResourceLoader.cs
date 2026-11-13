using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class ResourceLoader<TAsset> where TAsset : class
	{
		public static TAsset Load(ref ContentManifest.AssetEntry entry)
		{
			string resourcePath = ResourcePathResolver.Resolve(entry.Key, typeof(TAsset));
			UnityEngine.Object @object = Resources.Load(resourcePath, typeof(TAsset));
			if (@object == null)
			{
				throw new ArgumentException("Asset could not be loaded. Is the key correct? Key = " + entry.Key + ", resolved path = " + resourcePath);
			}
			return (TAsset)(object)@object;
		}
	}

	internal static class ResourcePathResolver
	{
		public static string Resolve(string key, Type assetType)
		{
			string orientationFolder = GetOrientationFolder();
			if (!string.IsNullOrEmpty(orientationFolder))
			{
				string orientationPath = orientationFolder + "/" + key;
				if (Resources.Load(orientationPath, assetType) != null)
				{
					return orientationPath;
				}
			}

			return key;
		}

		private static string GetOrientationFolder()
		{
#if UNITY_ANDROID || UNITY_IOS
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				return "resources_portrait";
			}
#endif
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
			case RuntimePlatform.IPhonePlayer:
				return "resources_portrait";
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.LinuxEditor:
				return "resources_landscape";
			default:
				return null;
			}
		}
	}
}
