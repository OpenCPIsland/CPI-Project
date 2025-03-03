using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class SpriteContentKey : TypedAssetContentKey<Sprite>
	{
		public SpriteContentKey()
		{
		}

		public SpriteContentKey(string key)
			: base(key)
		{
		}

		public SpriteContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
