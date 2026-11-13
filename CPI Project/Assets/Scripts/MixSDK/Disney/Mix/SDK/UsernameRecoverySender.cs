using DeviceDB;
using Disney.Mix.SDK.Internal;
using System;

namespace Disney.Mix.SDK
{
	public class UsernameRecoverySender : IUsernameRecoverySender
	{
		private readonly Disney.Mix.SDK.Internal.IUsernameRecoverySender usernameRecoverySender;

		public UsernameRecoverySender(IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string oneIdClientId, ICoroutineManager coroutineManager)
		{
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
			GuestControllerClient guestControllerClient = new GuestControllerClient(wwwCallFactory, guestControllerSpoofedIpAddress, database, "NoSWID", guestControllerHostUrl, oneIdClientId, logger);
			usernameRecoverySender = new Disney.Mix.SDK.Internal.UsernameRecoverySender(logger, guestControllerClient);
		}

		public void Send(string lookupValue, string languageCode, Action<ISendUsernameRecoveryResult> callback)
		{
			usernameRecoverySender.Send(lookupValue, languageCode, callback);
		}
	}
}
