using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class FontContentKey : TypedAssetContentKey<Font>
	{
		public FontContentKey()
		{
		}

		public FontContentKey(string key)
			: base(key)
		{
		}

		public FontContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
