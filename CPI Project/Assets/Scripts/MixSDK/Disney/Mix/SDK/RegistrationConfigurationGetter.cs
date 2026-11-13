using DeviceDB;
using Disney.Mix.SDK.Internal;
using System;

namespace Disney.Mix.SDK
{
	public class RegistrationConfigurationGetter : IRegistrationConfigurationGetter
	{
		private readonly Disney.Mix.SDK.Internal.IRegistrationConfigurationGetter registrationConfigurationGetter;

		public RegistrationConfigurationGetter(IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string oneIdClientId, ICoroutineManager coroutineManager, string mixApiHostUrl, string mixClientToken)
		{
			SystemStopwatchFactory stopwatchFactory = new SystemStopwatchFactory();
			SystemWwwFactory wwwFactory = new SystemWwwFactory();
			WwwCallFactory wwwCallFactory = new WwwCallFactory(logger, coroutineManager, stopwatchFactory, wwwFactory);
			JsonWebCallEncryptor webCallEncryptor = new JsonWebCallEncryptor();
			FileSystem fileSystem = new FileSystem();
			DatabaseDirectoryCreator directoryCreator = new DatabaseDirectoryCreator(fileSystem, localStorageDirPath);
			DocumentCollectionFactory documentCollectionFactory = new DocumentCollectionFactory();
			SystemRandom random = new SystemRandom();
			DatabaseCorruptionHandler databaseCorruptionHandler = new DatabaseCorruptionHandler(logger, fileSystem, localStorageDirPath);
			SystemEpochTime epochTime = new SystemEpochTime();
			Database database = new Database(keychain.LocalStorageKey, random, epochTime, directoryCreator, documentCollectionFactory, databaseCorruptionHandler);
			MixWebCallQueue webCallQueue = new MixWebCallQueue();
			NoOpSessionRefresher sessionRefresher = new NoOpSessionRefresher();
			MixWebCallFactory webCallFactory = new MixWebCallFactory(logger, mixApiHostUrl, wwwCallFactory, webCallEncryptor, string.Empty, string.Empty, mixClientToken, webCallQueue, sessionRefresher, epochTime, database);
			AgeBandBuilder ageBandBuilder = new AgeBandBuilder(logger, webCallFactory);
			GuestControllerClientFactory guestControllerClientFactory = new GuestControllerClientFactory(wwwCallFactory, guestControllerSpoofedIpAddress, database, guestControllerHostUrl, oneIdClientId, logger);
			registrationConfigurationGetter = new Disney.Mix.SDK.Internal.RegistrationConfigurationGetter(logger, guestControllerClientFactory, ageBandBuilder);
		}

		public void Get(Action<IGetRegistrationConfigurationResult> callback)
		{
			registrationConfigurationGetter.Get(delegate(IInternalGetRegistrationConfigurationResult r)
			{
				callback(r);
			});
		}

		public void Get(string countryCode, Action<IGetRegistrationConfigurationResult> callback)
		{
			registrationConfigurationGetter.Get(countryCode, delegate(IInternalGetRegistrationConfigurationResult r)
			{
				callback(r);
			});
		}
	}
}
