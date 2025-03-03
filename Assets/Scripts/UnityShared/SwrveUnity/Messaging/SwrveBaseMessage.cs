namespace SwrveUnity.Messaging
{
	public abstract class SwrveBaseMessage
	{
		public int Id;

		public SwrveBaseCampaign Campaign;

		public string GetBaseMessageType()
		{
			return GetBaseFormattedMessageType().ToLower();
		}

		public abstract string GetBaseFormattedMessageType();

		public string GetEventPrefix()
		{
			return "Swrve." + GetBaseFormattedMessageType() + "s." + GetBaseMessageType() + "_";
		}
	}
}
