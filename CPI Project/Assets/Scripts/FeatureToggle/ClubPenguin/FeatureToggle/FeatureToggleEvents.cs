namespace ClubPenguin.FeatureToggle
{
	public static class FeatureToggleEvents
	{
		public struct EnableFeature
		{
			public readonly FeatureDefinition Feature;

			public EnableFeature(FeatureDefinition feature)
			{
				Feature = feature;
			}
		}

		public struct DisableFeature
		{
			public readonly FeatureDefinition Feature;

			public DisableFeature(FeatureDefinition feature)
			{
				Feature = feature;
			}
		}
	}
}
