using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.FeatureToggle
{
	[Serializable]
	public class FeatureDefinitionKey : TypedStaticGameDataKey<FeatureDefinition, string>
	{
		public FeatureDefinitionKey(string id)
		{
			Id = id;
		}
	}
}
