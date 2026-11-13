using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class MusicGenreDefinitionDefinitionKey : TypedStaticGameDataKey<MusicGenreDefinition, int>
	{
		public MusicGenreDefinitionDefinitionKey(int id)
		{
			Id = id;
		}
	}
}
