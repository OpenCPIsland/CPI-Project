using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.DailyChallenge
{
	[Serializable]
	public class DailyChallengeDefinitionContentKey : TypedAssetContentKey<DailyChallengeDefinition>
	{
		public DailyChallengeDefinitionContentKey(string key)
			: base(key)
		{
		}
	}
}
