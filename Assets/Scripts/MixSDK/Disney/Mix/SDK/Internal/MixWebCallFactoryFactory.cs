namespace Disney.Mix.SDK.Internal
{
	public class MixWebCallFactoryFactory : IMixWebCallFactoryFactory
	{
		private readonly AbstractLogger logger;

		private readonly string hostUrl;

		private readonly string mixClientToken;

		private readonly IWwwCallFactory wwwCallFactory;

		private readonly IMixWebCallQueue webCallQueue;

		private readonly IEpochTime epochTime;

		private readonly IDatabase database;

		public MixWebCallFactoryFactory(AbstractLogger logger, string hostUrl, string mixClientToken, IWwwCallFactory wwwCallFactory, IMixWebCallQueue webCallQueue, IEpochTime epochTime, IDatabase database)
		{
			this.logger = logger;
			this.hostUrl = hostUrl;
			this.mixClientToken = mixClientToken;
			this.wwwCallFactory = wwwCallFactory;
			this.webCallQueue = webCallQueue;
			this.epochTime = epochTime;
			this.database = database;
		}

		public IMixWebCallFactory Create(IWebCallEncryptor webCallEncryptor, string swid, string guestControllerAccessToken, ISessionRefresher sessionRefresher)
		{
			return new MixWebCallFactory(logger, hostUrl, wwwCallFactory, webCallEncryptor, swid, guestControllerAccessToken, mixClientToken, webCallQueue, sessionRefresher, epochTime, database);
		}
	}
}
