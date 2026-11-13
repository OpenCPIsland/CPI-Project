namespace Disney.Mix.SDK.Internal
{
	public class UntrustedPushNotification : AbstractPushNotification, IUntrustedPushNotification, IPushNotification
	{
		public UntrustedPushNotification(bool notificationsAvailable)
			: base(notificationsAvailable)
		{
		}
	}
}
