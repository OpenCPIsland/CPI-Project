namespace Disney.Mix.SDK.Internal
{
	public interface IInternalPushNotification : IPushNotification
	{
		bool NotificationsAvailable
		{
			get;
		}
	}
}
