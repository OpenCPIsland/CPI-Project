namespace Disney.Mix.SDK.Internal
{
	public class GuestControllerClientFactory : IGuestControllerClientFactory
	{
		private readonly IWwwCallFactory wwwCallFactory;

		private readonly string spoofedIpAddress;

		private readonly IDatabase database;

		private readonly string host;

		private readonly string clientId;

		private readonly AbstractLogger logger;

		public GuestControllerClientFactory(IWwwCallFactory wwwCallFactory, string spoofedIpAddress, IDatabase database, string host, string clientId, AbstractLogger logger)
		{
			this.wwwCallFactory = wwwCallFactory;
			this.spoofedIpAddress = spoofedIpAddress;
			this.database = database;
			this.host = host;
			this.clientId = clientId;
			this.logger = logger;
		}

		public IGuestControllerClient Create(string swid)
		{
			return new GuestControllerClient(wwwCallFactory, spoofedIpAddress, database, swid, host, clientId, logger);
		}
	}
}
