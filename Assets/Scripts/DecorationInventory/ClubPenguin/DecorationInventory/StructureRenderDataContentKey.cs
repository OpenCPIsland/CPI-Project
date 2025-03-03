using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class StructureRenderDataContentKey : TypedAssetContentKey<StructureRenderData>
	{
		public StructureRenderDataContentKey(string key)
			: base(key)
		{
		}
	}
}
