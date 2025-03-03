namespace ClubPenguin
{
	public static class HeadStatusEvents
	{
		public struct ShowHeadStatus
		{
			public TemporaryHeadStatusType StatusType;

			public ShowHeadStatus(TemporaryHeadStatusType statusType)
			{
				StatusType = statusType;
			}
		}
	}
}
