namespace Disney.Mix.SDK.Internal
{
	public interface ISessionRefresherFactory
	{
		ISessionRefresher Create(IMixSessionStarter mixSessionStarter, IGuestControllerClient guestControllerClient);

		ISessionRefresher Create(IMixSessionStarter mixSessionStarter);
	}
}
