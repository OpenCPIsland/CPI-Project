using DeviceDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class SessionFactory : ISessionFactory
	{
		private readonly AbstractLogger logger;

		private readonly ICoroutineManager coroutineManager;

		private readonly IStopwatch pollCountdownStopwatch;

		private readonly IEpochTime epochTime;

		private readonly DatabaseCorruptionHandler databaseCorruptionHandler;

		private readonly INotificationQueue notificationQueue;

		private readonly INotificationDispatcher notificationDispatcher;

		private readonly ISessionStatus sessionStatus;

		private readonly IMixWebCallFactoryFactory mixWebCallFactoryFactory;

		private readonly IWebCallEncryptorFactory webCallEncryptorFactory;

		private readonly IMixSessionStarter mixSessionStarter;

		private readonly IKeychain keychain;

		private readonly ISessionRefresherFactory sessionRefresherFactory;

		private readonly IGuestControllerClientFactory guestControllerClientFactory;

		private readonly IRandom random;

		private readonly IEncryptor encryptor;

		private readonly IFileSystem fileSystem;

		private readonly IWwwCallFactory wwwCallFactory;

		private readonly string localStorageDirPath;

		private readonly string clientVersion;

		private readonly IDatabaseDirectoryCreator databaseDirectoryCreator;

		private readonly IDocumentCollectionFactory documentCollectionFactory;

		private readonly IDatabase database;

		public SessionFactory(AbstractLogger logger, ICoroutineManager coroutineManager, IStopwatch pollCountdownStopwatch, IEpochTime epochTime, DatabaseCorruptionHandler databaseCorruptionHandler, INotificationQueue notificationQueue, INotificationDispatcher notificationDispatcher, ISessionStatus sessionStatus, IMixWebCallFactoryFactory mixWebCallFactoryFactory, IWebCallEncryptorFactory webCallEncryptorFactory, IMixSessionStarter mixSessionStarter, IKeychain keychain, ISessionRefresherFactory sessionRefresherFactory, IGuestControllerClientFactory guestControllerClientFactory, IRandom random, IEncryptor encryptor, IFileSystem fileSystem, IWwwCallFactory wwwCallFactory, string localStorageDirPath, string clientVersion, IDatabaseDirectoryCreator databaseDirectoryCreator, IDocumentCollectionFactory documentCollectionFactory, IDatabase database)
		{
			this.logger = logger;
			this.coroutineManager = coroutineManager;
			this.pollCountdownStopwatch = pollCountdownStopwatch;
			this.epochTime = epochTime;
			this.databaseCorruptionHandler = databaseCorruptionHandler;
			this.notificationQueue = notificationQueue;
			this.notificationDispatcher = notificationDispatcher;
			this.sessionStatus = sessionStatus;
			this.mixWebCallFactoryFactory = mixWebCallFactoryFactory;
			this.webCallEncryptorFactory = webCallEncryptorFactory;
			this.mixSessionStarter = mixSessionStarter;
			this.keychain = keychain;
			this.sessionRefresherFactory = sessionRefresherFactory;
			this.guestControllerClientFactory = guestControllerClientFactory;
			this.random = random;
			this.encryptor = encryptor;
			this.fileSystem = fileSystem;
			this.wwwCallFactory = wwwCallFactory;
			this.localStorageDirPath = localStorageDirPath;
			this.clientVersion = clientVersion;
			this.databaseDirectoryCreator = databaseDirectoryCreator;
			this.documentCollectionFactory = documentCollectionFactory;
			this.database = database;
		}

		public IInternalSession Create(string swid)
		{
			byte[] localStorageKey = keychain.LocalStorageKey;
			IDocumentCollection<AlertDocument> documentCollection = GetDocumentCollection<AlertDocument>(swid, "Alerts", databaseDirectoryCreator, localStorageKey, documentCollectionFactory);
			IDocumentCollection<FriendDocument> documentCollection2 = GetDocumentCollection<FriendDocument>(swid, "Friends", databaseDirectoryCreator, localStorageKey, documentCollectionFactory);
			IDocumentCollection<FriendInvitationDocument> documentCollection3 = GetDocumentCollection<FriendInvitationDocument>(swid, "FriendInvitations", databaseDirectoryCreator, localStorageKey, documentCollectionFactory);
			IDocumentCollection<UserDocument> documentCollection4 = GetDocumentCollection<UserDocument>(swid, "Users", databaseDirectoryCreator, localStorageKey, documentCollectionFactory);
			databaseCorruptionHandler.Add(documentCollection4);
			string dirPath = BuildDocCollectionPath(databaseDirectoryCreator, swid);
			UserDatabase userDatabase = new UserDatabase(documentCollection, documentCollection2, documentCollection3, documentCollection4, localStorageKey, dirPath, epochTime, documentCollectionFactory, databaseCorruptionHandler, coroutineManager);
			database.ClearServerTimeOffsetMillis();
			epochTime.OffsetMilliseconds = (database.GetServerTimeOffsetMillis() ?? 0);
			logger.Debug("Initial time offset: " + epochTime.Offset);
			SessionDocument sessionDocument = database.GetSessionDocument(swid);
			keychain.PushNotificationKey = sessionDocument.CurrentSymmetricEncryptionKey;
			IWebCallEncryptor webCallEncryptor = webCallEncryptorFactory.Create(sessionDocument.CurrentSymmetricEncryptionKey, sessionDocument.SessionId);
			IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create(swid);
			ISessionRefresher sessionRefresher = sessionRefresherFactory.Create(mixSessionStarter, guestControllerClient);
			IMixWebCallFactory mixWebCallFactory = mixWebCallFactoryFactory.Create(webCallEncryptor, swid, sessionDocument.GuestControllerAccessToken, sessionRefresher);
			guestControllerClient.OnAccessTokenChanged += delegate(object sender, AbstractGuestControllerAccessTokenChangedEventArgs e)
			{
				mixWebCallFactory.GuestControllerAccessToken = e.GuestControllerAccessToken;
			};
			AssetLoader assetLoader = new AssetLoader(logger, wwwCallFactory);
			IList<IInternalFriend> friends = CreateFriends(userDatabase);
			AgeBandType ageBandType = AgeBandTypeConverter.Convert(sessionDocument.AgeBand);
			DateTime lastRefreshTime = epochTime.FromSeconds(sessionDocument.LastProfileRefreshTime);
			RegistrationProfile registrationProfile = new RegistrationProfile(logger, sessionDocument.DisplayNameText, sessionDocument.ProposedDisplayName, sessionDocument.ProposedDisplayNameStatus, sessionDocument.FirstName, sessionDocument.AccountStatus, lastRefreshTime, sessionDocument.CountryCode);
			GetStateResponseParser getStateResponseParser = new GetStateResponseParser(logger);
			NotificationPoller notificationPoller = new NotificationPoller(logger, mixWebCallFactory, notificationQueue, pollCountdownStopwatch, getStateResponseParser, epochTime, random, database, swid);
			DisplayName displayName = new DisplayName(sessionDocument.DisplayNameText);
			LocalUser localUser = new LocalUser(logger, displayName, swid, friends, ageBandType, database, userDatabase, registrationProfile, mixWebCallFactory, guestControllerClient, notificationQueue, encryptor, epochTime);
			Session session = new Session(logger, localUser, sessionDocument.GuestControllerAccessToken, sessionDocument.PushNotificationToken != null, notificationPoller, coroutineManager, database, userDatabase, guestControllerClient, mixWebCallFactory, epochTime, databaseCorruptionHandler, sessionStatus, keychain, getStateResponseParser, clientVersion, notificationQueue);
			try
			{
				NotificationHandler.Handle(notificationDispatcher, userDatabase, localUser, epochTime);
				notificationQueue.LatestSequenceNumber = sessionDocument.LatestNotificationSequenceNumber;
				IEnumerable<IncomingFriendInvitation> incomingFriendInvitations = GetIncomingFriendInvitations(userDatabase, localUser);
				foreach (IncomingFriendInvitation item in incomingFriendInvitations)
				{
					localUser.AddIncomingFriendInvitation(item);
				}
				IEnumerable<OutgoingFriendInvitation> outgoingFriendInvitations = GetOutgoingFriendInvitations(userDatabase, localUser);
				foreach (OutgoingFriendInvitation item2 in outgoingFriendInvitations)
				{
					localUser.AddOutgoingFriendInvitation(item2);
				}
			}
			catch (Exception)
			{
				session.Dispose();
				throw;
			}
			return session;
		}

		private static IEnumerable<IncomingFriendInvitation> GetIncomingFriendInvitations(IUserDatabase userDatabase, IInternalLocalUser localUser)
		{
			return (from friendInvitationDoc in userDatabase.GetFriendInvitationDocuments(false)
				where userDatabase.GetUserByDisplayName(friendInvitationDoc.DisplayName) != null
				select friendInvitationDoc).Select(delegate(FriendInvitationDocument friendInvitationDoc)
			{
				UserDocument userByDisplayName = userDatabase.GetUserByDisplayName(friendInvitationDoc.DisplayName);
				string displayName = userByDisplayName.DisplayName;
				IInternalUnidentifiedUser inviter = RemoteUserFactory.CreateUnidentifiedUser(displayName, userByDisplayName.FirstName, userDatabase);
				bool isTrusted = friendInvitationDoc.IsTrusted;
				IncomingFriendInvitation incomingFriendInvitation = new IncomingFriendInvitation(inviter, localUser, isTrusted);
				incomingFriendInvitation.SendComplete(friendInvitationDoc.FriendInvitationId);
				return incomingFriendInvitation;
			}).ToList();
		}

		private static IEnumerable<OutgoingFriendInvitation> GetOutgoingFriendInvitations(IUserDatabase userDatabase, IInternalLocalUser localUser)
		{
			return (from friendInvitationDoc in userDatabase.GetFriendInvitationDocuments(true)
				where userDatabase.GetUserByDisplayName(friendInvitationDoc.DisplayName) != null
				select friendInvitationDoc).Select(delegate(FriendInvitationDocument friendInvitationDoc)
			{
				UserDocument userByDisplayName = userDatabase.GetUserByDisplayName(friendInvitationDoc.DisplayName);
				string displayName = userByDisplayName.DisplayName;
				IInternalUnidentifiedUser invitee = RemoteUserFactory.CreateUnidentifiedUser(displayName, userByDisplayName.FirstName, userDatabase);
				bool isTrusted = friendInvitationDoc.IsTrusted;
				OutgoingFriendInvitation outgoingFriendInvitation = new OutgoingFriendInvitation(localUser, invitee, isTrusted);
				outgoingFriendInvitation.SendComplete(friendInvitationDoc.FriendInvitationId);
				return outgoingFriendInvitation;
			}).ToList();
		}

		private static IList<IInternalFriend> CreateFriends(IUserDatabase userDatabase)
		{
			return userDatabase.GetAllFriendDocuments().Select(delegate(FriendDocument friendDoc)
			{
				UserDocument userBySwid = userDatabase.GetUserBySwid(friendDoc.Swid);
				return RemoteUserFactory.CreateFriend(friendDoc.Swid, friendDoc.IsTrusted, userBySwid.DisplayName, userBySwid.FirstName, userDatabase);
			}).ToList();
		}

		private static IDocumentCollection<TDocument> GetDocumentCollection<TDocument>(string userSwid, string entityName, IDatabaseDirectoryCreator directoryCreator, byte[] encryptionKey, IDocumentCollectionFactory documentCollectionFactory) where TDocument : AbstractDocument, new()
		{
			string dir = BuildDocCollectionPath(directoryCreator, userSwid);
			string path = HashedPathGenerator.GetPath(dir, entityName);
			return documentCollectionFactory.CreateHighSecurityFileSystemCollection<TDocument>(path, encryptionKey);
		}

		private static string BuildDocCollectionPath(IDatabaseDirectoryCreator directoryCreator, string userSwid)
		{
			string dir = directoryCreator.CreateUserDirectory();
			return HashedPathGenerator.GetPath(dir, userSwid);
		}
	}
}
