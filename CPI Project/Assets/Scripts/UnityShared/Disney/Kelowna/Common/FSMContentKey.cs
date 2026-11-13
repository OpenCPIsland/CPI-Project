using System;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class FSMContentKey : TypedAssetContentKey<FsmTemplate>
	{
		public FSMContentKey()
		{
		}

		public FSMContentKey(string key)
			: base(key)
		{
		}

		public FSMContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
