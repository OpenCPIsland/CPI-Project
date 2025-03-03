using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class TagDefinitionKey : TypedStaticGameDataKey<TagDefinition, string>
	{
		public TagDefinitionKey(string id)
		{
			Id = id;
		}
	}
}
