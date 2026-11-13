using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class CustomButtonKey : TypedAssetContentKey<CustomButton>
	{
		public CustomButtonKey()
		{
		}

		public CustomButtonKey(string key)
			: base(key)
		{
		}

		public CustomButtonKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
