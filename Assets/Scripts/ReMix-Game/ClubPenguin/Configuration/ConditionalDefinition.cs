using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Configuration
{
	public class ConditionalDefinition : StaticGameDataDefinition
	{
		public bool SendAnalytics = true;

		public virtual ConditionalProperty GenerateProperty()
		{
			throw new InvalidOperationException("GenerateProperty must be overriden in sub class");
		}
	}
}
