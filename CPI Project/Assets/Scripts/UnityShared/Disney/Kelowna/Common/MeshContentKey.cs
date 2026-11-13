using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class MeshContentKey : TypedAssetContentKey<Mesh>
	{
		public MeshContentKey()
		{
		}

		public MeshContentKey(string key)
			: base(key)
		{
		}

		public MeshContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
