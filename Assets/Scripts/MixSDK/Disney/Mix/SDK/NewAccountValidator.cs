using DeviceDB;
using Disney.Mix.SDK.Internal;
using System;

namespace Disney.Mix.SDK
{
	public class NewAccountValidator : INewAccountValidator
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClient guestControllerClient;

		public NewAccountValidator(IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string oneIdClientId, ICoroutineManager coroutineManager)
		{
			this.logger = logger;
			SystemStopwatchFactory stopwatchFactory = new SystemStopwatchFactory();
			SystemWwwFactory wwwFactory = new SystemWwwFactory();
			WwwCallFactory wwwCallFactory = new WwwCallFactory(logger, coroutineManager, stopwatchFactory, wwwFactory);
			FileSystem fileSystem = new FileSystem();
			DatabaseDirectoryCreator directoryCreator = new DatabaseDirectoryCreator(fileSystem, localStorageDirPath);
			DocumentCollectionFactory documentCollectionFactory = new DocumentCollectionFactory();
			SystemRandom random = new SystemRandom();
			DatabaseCorruptionHandler databaseCorruptionHandler = new DatabaseCorruptionHandler(logger, fileSystem, localStorageDirPath);
			SystemEpochTime epochTime = new SystemEpochTime();
			Database database = new Database(keychain.LocalStorageKey, random, epochTime, directoryCreator, documentCollectionFactory, databaseCorruptionHandler);
			guestControllerClient = new GuestControllerClient(wwwCallFactory, guestControllerSpoofedIpAddress, database, string.Empty, guestControllerHostUrl, oneIdClientId, logger);
		}

		public void ValidateAdultAccount(string email, string password, Action<IValidateNewAccountResult> callback)
		{
			GuestControllerAccountValidator.ValidateAdultAccount(logger, guestControllerClient, email, password, callback);
		}

		public void ValidateChildAccount(string username, string password, Action<IValidateNewAccountResult> callback)
		{
			GuestControllerAccountValidator.ValidateChildAccount(logger, guestControllerClient, username, password, callback);
		}
	}
}
