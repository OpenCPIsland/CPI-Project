using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationCategoryDefinitionDefinitionKey : TypedStaticGameDataKey<DecorationCategoryDefinition, int>
	{
		public DecorationCategoryDefinitionDefinitionKey(int id)
		{
			Id = id;
		}
	}
}
