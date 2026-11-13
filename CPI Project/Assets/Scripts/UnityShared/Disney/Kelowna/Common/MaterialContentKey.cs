using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class MaterialContentKey : TypedAssetContentKey<Material>
	{
		public MaterialContentKey()
		{
		}

		public MaterialContentKey(string key)
			: base(key)
		{
		}

		public MaterialContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
