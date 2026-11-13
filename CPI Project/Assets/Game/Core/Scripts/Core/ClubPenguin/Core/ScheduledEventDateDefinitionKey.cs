using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class ScheduledEventDateDefinitionKey : TypedStaticGameDataKey<ScheduledEventDateDefinition, int>
	{
		public ScheduledEventDateDefinitionKey(int id)
		{
			Id = id;
		}
	}
}
