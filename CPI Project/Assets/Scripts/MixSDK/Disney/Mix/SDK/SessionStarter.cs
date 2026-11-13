using DeviceDB;
using Disney.Mix.SDK.Internal;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public class SessionStarter : ISessionStarter
	{
		public void Login(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string loginValue, string password, string languageCode, Action<ILoginResult> callback)
		{
			ISessionLogin sessionLogin;
			ISessionRegister sessionRegister;
			ISessionRestorer sessionRestorer;
			ISessionReuser sessionReuser;
			IOfflineSessionCreator offlineSessionCreator;
			CreateDependencies(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, languageCode, out sessionLogin, out sessionRegister, out sessionRestorer, out sessionReuser, out offlineSessionCreator);
			sessionLogin.Login(loginValue, password, callback);
		}

		public void RegisterAdultAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string password, string firstName, string lastName, string email, string languageCode, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			Register(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, isTestProfile, null, password, firstName, lastName, email, null, languageCode, null, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		public void RegisterAdultAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string password, string firstName, string lastName, string email, string languageCode, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			Register(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, isTestProfile, null, password, firstName, lastName, email, null, languageCode, assertedCountry, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		public void RegisterChildAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string username, string password, string firstName, string parentEmail, string languageCode, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			Register(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, isTestProfile, username, password, firstName, null, null, parentEmail, languageCode, null, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		public void RegisterChildAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string username, string password, string firstName, string parentEmail, string languageCode, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			Register(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, isTestProfile, username, password, firstName, null, null, parentEmail, languageCode, assertedCountry, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		private static void Register(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string username, string password, string firstName, string lastName, string email, string parentEmail, string languageCode, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			ISessionLogin sessionLogin;
			ISessionRegister sessionRegister;
			ISessionRestorer sessionRestorer;
			ISessionReuser sessionReuser;
			IOfflineSessionCreator offlineSessionCreator;
			CreateDependencies(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, languageCode, out sessionLogin, out sessionRegister, out sessionRestorer, out sessionReuser, out offlineSessionCreator);
			sessionRegister.Register(isTestProfile, username, password, firstName, lastName, email, parentEmail, languageCode, assertedCountry, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		public void RestoreLastSession(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string languageCode, Action<IRestoreLastSessionResult> callback)
		{
			ISessionLogin sessionLogin;
			ISessionRegister sessionRegister;
			ISessionRestorer sessionRestorer;
			ISessionReuser sessionReuser;
			IOfflineSessionCreator offlineSessionCreator;
			CreateDependencies(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, languageCode, out sessionLogin, out sessionRegister, out sessionRestorer, out sessionReuser, out offlineSessionCreator);
			sessionRestorer.RestoreLastSession(callback);
		}

		public void ReuseExistingGuestControllerLogin(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string swid, string accessToken, uint accessTokenTtl, string refreshToken, uint refreshTokenTtl, string displayName, string displayNameModeratedStatusDate, string proposedDisplayName, string displayNameProposedStatus, string etag, string ageBand, bool ageBandAssumed, string accountStatus, string dateOfBirth, string email, bool emailVerified, string firstName, string lastName, string middleName, string parentEmail, bool parentEmailVerified, string username, string languageCode, string countryCode, IEnumerable<KeyValuePair<string, bool>> marketingItems, Action<IReuseExistingGuestControllerLoginResult> callback)
		{
			ISessionLogin sessionLogin;
			ISessionRegister sessionRegister;
			ISessionRestorer sessionRestorer;
			ISessionReuser sessionReuser;
			IOfflineSessionCreator offlineSessionCreator;
			CreateDependencies(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, languageCode, out sessionLogin, out sessionRegister, out sessionRestorer, out sessionReuser, out offlineSessionCreator);
			sessionReuser.Reuse(swid, accessToken, refreshToken, displayName, proposedDisplayName, displayNameProposedStatus, firstName, etag, ageBand, accountStatus, countryCode, callback);
		}

		public void OfflineLastSession(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string languageCode, Action<IOfflineLastSessionResult> callback)
		{
			ISessionLogin sessionLogin;
			ISessionRegister sessionRegister;
			ISessionRestorer sessionRestorer;
			ISessionReuser sessionReuser;
			IOfflineSessionCreator offlineSessionCreator;
			CreateDependencies(guestControllerHostUrl, guestControllerSpoofedIpAddress, mixApiHostUrl, oneIdClientId, mixClientToken, clientVersion, coroutineManager, keychain, logger, localStorageDirPath, languageCode, out sessionLogin, out sessionRegister, out sessionRestorer, out sessionReuser, out offlineSessionCreator);
			IInternalOfflineLastSessionResult obj = offlineSessionCreator.Create();
			callback(obj);
		}

		private static void CreateDependencies(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string languageCode, out ISessionLogin sessionLogin, out ISessionRegister sessionRegister, out ISessionRestorer sessionRestorer, out ISessionReuser sessionReuser, out IOfflineSessionCreator offlineSessionCreator)
		{
			SystemStopwatchFactory systemStopwatchFactory = new SystemStopwatchFactory();
			SystemWwwFactory wwwFactory = new SystemWwwFactory();
			WwwCallFactory wwwCallFactory = new WwwCallFactory(logger, coroutineManager, systemStopwatchFactory, wwwFactory);
			FileSystem fileSystem = new FileSystem();
			DatabaseDirectoryCreator databaseDirectoryCreator = new DatabaseDirectoryCreator(fileSystem, localStorageDirPath);
			DocumentCollectionFactory documentCollectionFactory = new DocumentCollectionFactory();
			string sdkDatabasesDirectory = databaseDirectoryCreator.GetSdkDatabasesDirectory();
			DatabaseCorruptionHandler databaseCorruptionHandler = new DatabaseCorruptionHandler(logger, fileSystem, sdkDatabasesDirectory);
			SystemRandom random = new SystemRandom();
			SystemEpochTime epochTime = new SystemEpochTime();
			Database database = new Database(keychain.LocalStorageKey, random, epochTime, databaseDirectoryCreator, documentCollectionFactory, databaseCorruptionHandler);
			MixWebCallQueue webCallQueue = new MixWebCallQueue();
			MixSessionStartWebCallEncryptor sessionStartEncryptor = new MixSessionStartWebCallEncryptor();
			MixWebCallFactoryFactory mixWebCallFactoryFactory = new MixWebCallFactoryFactory(logger, mixApiHostUrl, mixClientToken, wwwCallFactory, webCallQueue, epochTime, database);
			NotificationDispatcher notificationDispatcher = new NotificationDispatcher();
			NotificationQueue notificationQueue = new NotificationQueue(notificationDispatcher);
			SessionStatus sessionStatus = new SessionStatus(true);
			NoOpSessionRefresher sessionRefresher = new NoOpSessionRefresher();
			JsonWebCallEncryptor webCallEncryptor = new JsonWebCallEncryptor();
			IMixWebCallFactory webCallFactory = mixWebCallFactoryFactory.Create(webCallEncryptor, string.Empty, string.Empty, sessionRefresher);
			RsaEncryptor rsaEncryptor = new RsaEncryptor();
			MixEncryptor encryptor = new MixEncryptor();
			MixWebCallEncryptorFactory webCallEncryptorFactory = new MixWebCallEncryptorFactory(encryptor);
			SessionRefresherFactory sessionRefresherFactory = new SessionRefresherFactory(webCallQueue);
			MixSessionStarter mixSessionStarter = new MixSessionStarter(logger, rsaEncryptor, database, webCallEncryptorFactory, sessionStartEncryptor, mixWebCallFactoryFactory, keychain, coroutineManager, sessionRefresherFactory);
			IStopwatch pollCountdownStopwatch = systemStopwatchFactory.Create();
			GuestControllerClientFactory guestControllerClientFactory = new GuestControllerClientFactory(wwwCallFactory, guestControllerSpoofedIpAddress, database, guestControllerHostUrl, oneIdClientId, logger);
			SessionFactory sessionFactory = new SessionFactory(logger, coroutineManager, pollCountdownStopwatch, epochTime, databaseCorruptionHandler, notificationQueue, notificationDispatcher, sessionStatus, mixWebCallFactoryFactory, webCallEncryptorFactory, mixSessionStarter, keychain, sessionRefresherFactory, guestControllerClientFactory, random, encryptor, fileSystem, wwwCallFactory, localStorageDirPath, clientVersion, databaseDirectoryCreator, documentCollectionFactory, database);
			AgeBandBuilder ageBandBuilder = new AgeBandBuilder(logger, webCallFactory);
			Disney.Mix.SDK.Internal.RegistrationConfigurationGetter registrationConfigurationGetter = new Disney.Mix.SDK.Internal.RegistrationConfigurationGetter(logger, guestControllerClientFactory, ageBandBuilder);
			LegalMarketingErrorsBuilder legalMarketingErrorsBuilder = new LegalMarketingErrorsBuilder(registrationConfigurationGetter, languageCode, epochTime);
			sessionLogin = new SessionLogin(logger, guestControllerClientFactory, mixSessionStarter, database, legalMarketingErrorsBuilder, sessionFactory);
			sessionRegister = new SessionRegister(logger, guestControllerClientFactory, database, mixSessionStarter, sessionFactory);
			sessionRestorer = new SessionRestorer(logger, guestControllerClientFactory, database, sessionFactory);
			sessionReuser = new SessionReuser(logger, database, mixSessionStarter, sessionFactory);
			offlineSessionCreator = new OfflineSessionCreator(logger, sessionFactory, database);
		}
	}
}
