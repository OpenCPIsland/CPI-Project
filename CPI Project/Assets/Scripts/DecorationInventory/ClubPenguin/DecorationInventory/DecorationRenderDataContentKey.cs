using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationRenderDataContentKey : TypedAssetContentKey<DecorationRenderData>
	{
		public DecorationRenderDataContentKey(string key)
			: base(key)
		{
		}
	}
}
