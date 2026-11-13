using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Tutorial
{
	[Serializable]
	public class TutorialDefinitionKey : TypedStaticGameDataKey<TutorialDefinition, int>
	{
		public TutorialDefinitionKey(int id)
		{
			Id = id;
		}
	}
}
