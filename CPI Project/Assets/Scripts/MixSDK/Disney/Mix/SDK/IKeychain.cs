namespace Disney.Mix.SDK
{
	public interface IKeychain
	{
		byte[] LocalStorageKey
		{
			get;
		}

		byte[] PushNotificationKey
		{
			set;
		}
	}
}
