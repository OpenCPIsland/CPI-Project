namespace Disney.Mix.SDK.Internal
{
	public class SessionRefresherFactory : ISessionRefresherFactory
	{
		private readonly IMixWebCallQueue webCallQueue;

		private IGuestControllerClient guestControllerClient;

		public SessionRefresherFactory(IMixWebCallQueue webCallQueue)
		{
			this.webCallQueue = webCallQueue;
		}

		public ISessionRefresher Create(IMixSessionStarter mixSessionStarter, IGuestControllerClient guestControllerClient)
		{
			this.guestControllerClient = guestControllerClient;
			return new SessionRefresher(webCallQueue, guestControllerClient, mixSessionStarter);
		}

		public ISessionRefresher Create(IMixSessionStarter mixSessionStarter)
		{
			return new SessionRefresher(webCallQueue, guestControllerClient, mixSessionStarter);
		}
	}
}
