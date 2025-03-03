using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class Texture2DContentKey : TypedAssetContentKey<Texture2D>
	{
		public Texture2DContentKey()
		{
		}

		public Texture2DContentKey(string key)
			: base(key)
		{
		}

		public Texture2DContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
