using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class PrefabContentKey : TypedAssetContentKey<GameObject>
	{
		public PrefabContentKey()
		{
		}

		public PrefabContentKey(string key)
			: base(key)
		{
		}

		public PrefabContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
