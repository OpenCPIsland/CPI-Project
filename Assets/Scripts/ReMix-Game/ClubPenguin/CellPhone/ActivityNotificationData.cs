namespace ClubPenguin.CellPhone
{
	public class ActivityNotificationData
	{
		public int CoinReward;

		public CellPhoneActivityDefinition Definition;

		public ActivityNotificationData(int coinReward, CellPhoneActivityDefinition definition)
		{
			CoinReward = coinReward;
			Definition = definition;
		}
	}
}
