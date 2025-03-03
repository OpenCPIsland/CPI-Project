namespace ClubPenguin.UI
{
	public static class ActionIndicatorEvents
	{
		public struct AddActionIndicator
		{
			public readonly DActionIndicator IndicatorData;

			public AddActionIndicator(DActionIndicator indicatorData)
			{
				IndicatorData = indicatorData;
			}
		}

		public struct RemoveActionIndicator
		{
			public readonly DActionIndicator IndicatorData;

			public RemoveActionIndicator(DActionIndicator indicatorData)
			{
				IndicatorData = indicatorData;
			}
		}
	}
}
