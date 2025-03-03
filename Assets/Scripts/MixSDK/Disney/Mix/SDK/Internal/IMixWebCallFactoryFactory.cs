namespace Disney.Mix.SDK.Internal
{
	public interface IMixWebCallFactoryFactory
	{
		IMixWebCallFactory Create(IWebCallEncryptor webCallEncryptor, string swid, string guestControllerAccessToken, ISessionRefresher sessionRefresher);
	}
}
