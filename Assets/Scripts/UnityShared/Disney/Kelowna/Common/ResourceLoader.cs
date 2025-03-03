using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class ResourceLoader<TAsset> where TAsset : class
	{
		public static TAsset Load(ref ContentManifest.AssetEntry entry)
		{
			UnityEngine.Object @object = Resources.Load(entry.Key, typeof(TAsset));
			if (@object == null)
			{
				throw new ArgumentException("Asset could not be loaded. Is the key correct? Key = " + entry.Key);
			}
			return (TAsset)(object)@object;
		}
	}
}
