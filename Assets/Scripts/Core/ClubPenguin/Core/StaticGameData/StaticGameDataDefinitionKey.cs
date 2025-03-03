using System;

namespace ClubPenguin.Core.StaticGameData
{
	[Serializable]
	public class StaticGameDataDefinitionKey : TypedStaticGameDataKey<StaticGameDataDefinition, string>
	{
		public StaticGameDataDefinitionKey(string id)
		{
			Id = id;
		}
	}
}
