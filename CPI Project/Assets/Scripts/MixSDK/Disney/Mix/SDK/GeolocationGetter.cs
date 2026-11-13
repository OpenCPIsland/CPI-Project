using DeviceDB;
using Disney.Mix.SDK.Internal;
using System;

namespace Disney.Mix.SDK
{
	public class GeolocationGetter : IGeolocationGetter
	{
		private readonly Disney.Mix.SDK.Internal.GeolocationGetter geolocationGetter;

		public GeolocationGetter(AbstractLogger logger, ICoroutineManager coroutineManager, string mixApiHostUrl, string mixClientToken, string localStorageDirPath, IKeychain keychain)
		{
			SystemStopwatchFactory stopwatchFactory = new SystemStopwatchFactory();
			SystemWwwFactory wwwFactory = new SystemWwwFactory();
			WwwCallFactory wwwCallFactory = new WwwCallFactory(logger, coroutineManager, stopwatchFactory, wwwFactory);
			JsonWebCallEncryptor webCallEncryptor = new JsonWebCallEncryptor();
			MixWebCallQueue webCallQueue = new MixWebCallQueue();
			NoOpSessionRefresher sessionRefresher = new NoOpSessionRefresher();
			FileSystem fileSystem = new FileSystem();
			DatabaseDirectoryCreator databaseDirectoryCreator = new DatabaseDirectoryCreator(fileSystem, localStorageDirPath);
			string sdkDatabasesDirectory = databaseDirectoryCreator.GetSdkDatabasesDirectory();
			DocumentCollectionFactory documentCollectionFactory = new DocumentCollectionFactory();
			DatabaseCorruptionHandler databaseCorruptionHandler = new DatabaseCorruptionHandler(logger, fileSystem, sdkDatabasesDirectory);
			SystemRandom random = new SystemRandom();
			SystemEpochTime epochTime = new SystemEpochTime();
			Database database = new Database(keychain.LocalStorageKey, random, epochTime, databaseDirectoryCreator, documentCollectionFactory, databaseCorruptionHandler);
			MixWebCallFactory mixWebCallFactory = new MixWebCallFactory(logger, mixApiHostUrl, wwwCallFactory, webCallEncryptor, string.Empty, string.Empty, mixClientToken, webCallQueue, sessionRefresher, epochTime, database);
			geolocationGetter = new Disney.Mix.SDK.Internal.GeolocationGetter(logger, mixWebCallFactory);
		}

		public void Get(Action<IGetGeolocationResult> callback)
		{
			geolocationGetter.Get(callback);
		}
	}
}
