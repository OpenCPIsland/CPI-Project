using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class LocalizedSpriteAssetContentKey : TypedAssetContentKey<Sprite>
	{
		public LocalizedSpriteAssetContentKey()
		{
		}

		public LocalizedSpriteAssetContentKey(string key)
			: base(key)
		{
		}

		public LocalizedSpriteAssetContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
