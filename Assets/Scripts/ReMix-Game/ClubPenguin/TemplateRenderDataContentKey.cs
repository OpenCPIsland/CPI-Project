using Disney.Kelowna.Common;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class TemplateRenderDataContentKey : TypedAssetContentKey<TemplateRenderData>
	{
		public TemplateRenderDataContentKey(string key)
			: base(key)
		{
		}
	}
}
