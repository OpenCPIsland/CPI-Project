using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class PromptDefinitionKey : TypedStaticGameDataKey<PromptDefinition, string>
	{
		public PromptDefinitionKey(string id)
		{
			Id = id;
		}
	}
}
