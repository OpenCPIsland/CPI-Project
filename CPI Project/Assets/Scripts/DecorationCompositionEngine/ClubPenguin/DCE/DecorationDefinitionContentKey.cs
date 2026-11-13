using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.DCE
{
	[Serializable]
	public class DecorationDefinitionContentKey : TypedAssetContentKey<DecorationDefinition>
	{
		public DecorationDefinitionContentKey()
		{
		}

		public DecorationDefinitionContentKey(string key)
			: base(key)
		{
		}

		public DecorationDefinitionContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
