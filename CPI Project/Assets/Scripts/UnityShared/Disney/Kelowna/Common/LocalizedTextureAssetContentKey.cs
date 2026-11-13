using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class LocalizedTextureAssetContentKey : TypedAssetContentKey<Texture>
	{
		public LocalizedTextureAssetContentKey()
		{
		}

		public LocalizedTextureAssetContentKey(string key)
			: base(key)
		{
		}

		public LocalizedTextureAssetContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
