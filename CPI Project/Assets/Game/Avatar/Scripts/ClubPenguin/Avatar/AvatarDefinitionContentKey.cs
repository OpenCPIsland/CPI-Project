using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class AvatarDefinitionContentKey : TypedAssetContentKey<AvatarDefinition>
	{
		public AvatarDefinitionContentKey()
		{
		}

		public AvatarDefinitionContentKey(string key)
			: base(key)
		{
		}

		public AvatarDefinitionContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
