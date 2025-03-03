namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class TogglePushNotificationRequest : BaseUserRequest
	{
		public PushToken PushToken;

		public string State;

		public string IosProvisioningId;
	}
}
