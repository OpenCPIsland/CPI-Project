namespace Disney.MobileNetwork
{
	public interface IPlugin : IInitializable
	{
		LoggerHelper Logger
		{
			get;
		}

		void SetLogger(LoggerHelper.LoggerDelegate loggerMessageHandler);
	}
}
