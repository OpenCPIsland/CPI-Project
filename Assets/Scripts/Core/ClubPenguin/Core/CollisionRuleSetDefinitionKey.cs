using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class CollisionRuleSetDefinitionKey : TypedStaticGameDataKey<CollisionRuleSetDefinition, string>
	{
		public CollisionRuleSetDefinitionKey(string id)
		{
			Id = id;
		}

		public override string ToString()
		{
			return "[CollisionRuleSetDefinitionKey]: " + Id;
		}
	}
}
