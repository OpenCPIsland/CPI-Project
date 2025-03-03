using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class GroupDefinitionKey : TypedAssetContentKey<GroupDefinition>
	{
		public GroupDefinitionKey(string key)
			: base(key)
		{
		}
	}
}
