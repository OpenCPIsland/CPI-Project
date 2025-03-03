using DeviceDB;
using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class UserDatabase : IUserDatabase, IDisposable
	{
		private readonly IDocumentCollection<AlertDocument> alerts;

		private readonly IDocumentCollection<FriendDocument> friends;

		private readonly IDocumentCollection<FriendInvitationDocument> friendInvitations;

		private readonly IDocumentCollection<UserDocument> users;

		private readonly byte[] encryptionKey;

		private readonly string dirPath;

		private readonly IEpochTime epochTime;

		private readonly IDocumentCollectionFactory documentCollectionFactory;

		private readonly DatabaseCorruptionHandler databaseCorruptionHandler;

		private readonly ICoroutineManager coroutineManager;

		private readonly IDictionary<long, Action> callbacks;

		private readonly IList<long> pendingIndexedThreads;

		private bool isIndexingThread;

		public UserDatabase(IDocumentCollection<AlertDocument> alerts, IDocumentCollection<FriendDocument> friends, IDocumentCollection<FriendInvitationDocument> friendInvitations, IDocumentCollection<UserDocument> users, byte[] encryptionKey, string dirPath, IEpochTime epochTime, IDocumentCollectionFactory documentCollectionFactory, DatabaseCorruptionHandler databaseCorruptionHandler, ICoroutineManager coroutineManager)
		{
			this.alerts = alerts;
			this.friends = friends;
			this.friendInvitations = friendInvitations;
			this.users = users;
			this.encryptionKey = encryptionKey;
			this.dirPath = dirPath;
			this.epochTime = epochTime;
			this.documentCollectionFactory = documentCollectionFactory;
			this.databaseCorruptionHandler = databaseCorruptionHandler;
			this.coroutineManager = coroutineManager;
			callbacks = new Dictionary<long, Action>();
			pendingIndexedThreads = new List<long>();
		}

		public void Dispose()
		{
			alerts.Dispose();
			friends.Dispose();
			friendInvitations.Dispose();
			users.Dispose();
		}

		public IInternalAlert GetAlert(long alertId)
		{
			AlertDocument alertDocument = GetAlertDocument(alertId);
			return new Alert(alertDocument);
		}

		private AlertDocument GetAlertDocument(long alertId)
		{
			try
			{
				return (from id in alerts.FindDocumentIdsEqual(AlertDocument.AlertIdFieldName, alertId)
					select alerts.Find(id)).FirstOrDefault();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public IList<IInternalAlert> GetAlerts()
		{
			try
			{
				return ((IEnumerable<AlertDocument>)alerts).Select((Func<AlertDocument, IInternalAlert>)((AlertDocument doc) => new Alert(doc))).ToList();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void AddAlert(IInternalAlert alert)
		{
			try
			{
				string type = AlertTypeUtils.ToString(alert.Type);
				string level = alert.Level.ToString();
				AlertDocument alertDocument = GetAlertDocument(alert.AlertId);
				if (alertDocument == null)
				{
					AlertDocument alertDocument2 = new AlertDocument();
					alertDocument2.AlertId = alert.AlertId;
					alertDocument2.Type = type;
					alertDocument2.Level = level;
					alertDocument = alertDocument2;
					alerts.Insert(alertDocument);
				}
				else
				{
					alertDocument.Type = type;
					alertDocument.Level = level;
					alerts.Update(alertDocument);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void RemoveAlert(long alertId)
		{
			try
			{
				uint[] array = EnumerateAlertIds(alertId).ToArray();
				foreach (uint documentId in array)
				{
					alerts.Delete(documentId);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		private IEnumerable<uint> EnumerateAlertIds(long alertId)
		{
			return alerts.FindDocumentIdsEqual(AlertDocument.AlertIdFieldName, alertId);
		}

		public FriendDocument[] GetAllFriendDocuments()
		{
			try
			{
				return friends.ToArray();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void DeleteFriend(string swid)
		{
			try
			{
				uint[] array = EnumerateFriendDocumentIds(friends, swid).ToArray();
				foreach (uint documentId in array)
				{
					friends.Delete(documentId);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void SetFriendIsTrusted(string swid, bool isTrusted)
		{
			try
			{
				UpdateFriend(friends, swid, delegate(FriendDocument doc)
				{
					doc.IsTrusted = isTrusted;
				});
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void InsertFriend(FriendDocument doc)
		{
			try
			{
				if (!Contains(friends, doc.Swid))
				{
					friends.Insert(doc);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		private static void UpdateFriend(IDocumentCollection<FriendDocument> friends, string swid, Action<FriendDocument> update)
		{
			FriendDocument[] array = (from id in EnumerateFriendDocumentIds(friends, swid)
				select friends.Find(id)).ToArray();
			foreach (FriendDocument friendDocument in array)
			{
				update(friendDocument);
				friends.Update(friendDocument);
			}
		}

		public void ClearFriends()
		{
			try
			{
				friends.Drop();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		private static IEnumerable<uint> EnumerateFriendDocumentIds(IDocumentCollection<FriendDocument> friends, string swid)
		{
			return friends.FindDocumentIdsEqual(FriendDocument.SwidFieldName, swid);
		}

		private static bool Contains(IDocumentCollection<FriendDocument> friends, string swid)
		{
			return friends.FindDocumentIdsEqual(FriendDocument.SwidFieldName, swid).Any();
		}

		public void DeleteFriendInvitation(long invitationId)
		{
			try
			{
				uint[] array = friendInvitations.FindDocumentIdsEqual(FriendInvitationDocument.FriendInvitationIdFieldName, invitationId).ToArray();
				uint[] array2 = array;
				foreach (uint documentId in array2)
				{
					friendInvitations.Delete(documentId);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void InsertFriendInvitation(FriendInvitationDocument doc)
		{
			try
			{
				friendInvitations.Insert(doc);
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void InsertOrUpdateFriendInvitation(FriendInvitationDocument doc)
		{
			try
			{
				FriendInvitationDocument friendInvitationDocument = (from id in friendInvitations.FindDocumentIdsEqual(FriendInvitationDocument.FriendInvitationIdFieldName, doc.FriendInvitationId)
					select friendInvitations.Find(id)).FirstOrDefault();
				if (friendInvitationDocument == null)
				{
					friendInvitations.Insert(doc);
				}
				else
				{
					doc.Id = friendInvitationDocument.Id;
					friendInvitations.Update(doc);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void ClearFriendInvitations()
		{
			try
			{
				friendInvitations.Drop();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public FriendInvitationDocument[] GetFriendInvitationDocuments(bool isInviter)
		{
			try
			{
				return friendInvitations.Where((FriendInvitationDocument friendInvitationDoc) => friendInvitationDoc.IsInviter == isInviter).ToArray();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public UserDocument GetUserBySwid(string swid)
		{
			try
			{
				return (swid == null) ? null : (from id in users.FindDocumentIdsEqual(UserDocument.SwidFieldName, swid)
					select users.Find(id)).FirstOrDefault();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public UserDocument GetUserByDisplayName(string displayName)
		{
			try
			{
				return (displayName == null) ? null : (from id in users.FindDocumentIdsEqual(UserDocument.DisplayNameFieldName, displayName)
					select users.Find(id)).FirstOrDefault();
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void InsertUserDocument(UserDocument userDocument)
		{
			try
			{
				users.Insert(userDocument);
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void UpdateUserDocument(UserDocument userDocument)
		{
			try
			{
				users.Update(userDocument);
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void PersistUser(string swid, string hashedSwid, string displayName, string firstName, string status)
		{
			UserDocument userDocumentBySwidOrDisplayName = GetUserDocumentBySwidOrDisplayName(swid, displayName);
			if (userDocumentBySwidOrDisplayName == null)
			{
				UserDocument userDocument = new UserDocument();
				userDocument.Swid = swid;
				userDocument.HashedSwid = hashedSwid;
				userDocument.DisplayName = displayName;
				userDocument.FirstName = firstName;
				userDocument.Status = status;
				userDocumentBySwidOrDisplayName = userDocument;
				InsertUserDocument(userDocumentBySwidOrDisplayName);
				return;
			}
			bool flag = false;
			if (swid != null && swid != userDocumentBySwidOrDisplayName.Swid)
			{
				userDocumentBySwidOrDisplayName.Swid = swid;
				flag = true;
			}
			if (hashedSwid != null && hashedSwid != userDocumentBySwidOrDisplayName.HashedSwid)
			{
				userDocumentBySwidOrDisplayName.HashedSwid = hashedSwid;
				flag = true;
			}
			if (displayName != null && displayName != userDocumentBySwidOrDisplayName.DisplayName)
			{
				userDocumentBySwidOrDisplayName.DisplayName = displayName;
				flag = true;
			}
			if (firstName != null && firstName != userDocumentBySwidOrDisplayName.FirstName)
			{
				userDocumentBySwidOrDisplayName.FirstName = firstName;
				flag = true;
			}
			if (status != null && status != userDocumentBySwidOrDisplayName.Status)
			{
				userDocumentBySwidOrDisplayName.Status = status;
				flag = true;
			}
			if (flag)
			{
				UpdateUserDocument(userDocumentBySwidOrDisplayName);
			}
		}

		private UserDocument GetUserDocumentBySwidOrDisplayName(string swid, string displayName)
		{
			UserDocument userDocument = null;
			if (swid != null)
			{
				userDocument = GetUserBySwid(swid);
			}
			if (userDocument == null && displayName != null)
			{
				userDocument = GetUserByDisplayName(displayName);
			}
			return userDocument;
		}

		public void SyncToGetStateResponse(GetStateResponse response, Action callback)
		{
			if (response.Alerts != null)
			{
				foreach (Disney.Mix.SDK.Internal.MixDomain.Alert alert in response.Alerts)
				{
					AddAlert(new Alert(alert));
				}
			}
			ClearFriends();
			if (response.Friendships != null)
			{
				foreach (Friendship friendship in response.Friendships)
				{
					PersistUser(friendship.FriendUserId, null, null, null, null);
					FriendDocument friendDocument = new FriendDocument();
					friendDocument.Swid = friendship.FriendUserId;
					friendDocument.IsTrusted = friendship.IsTrusted.Value;
					FriendDocument doc = friendDocument;
					InsertFriend(doc);
				}
			}
			ClearFriendInvitations();
			if (response.FriendshipInvitations != null)
			{
				foreach (FriendshipInvitation friendshipInvitation in response.FriendshipInvitations)
				{
					List<User> source = response.Users;
					Func<User, bool> predicate = (User user) => user.DisplayName == friendshipInvitation.FriendDisplayName;
					User user2 = source.FirstOrDefault(predicate);
					if (user2 != null)
					{
						string firstName = user2.FirstName;
						string userId = user2.UserId;
						string hashedUserId = user2.HashedUserId;
						string status = user2.Status;
						PersistUser(userId, hashedUserId, friendshipInvitation.FriendDisplayName, firstName, status);
					}
					FriendInvitationDocument friendInvitationDocument = new FriendInvitationDocument();
					friendInvitationDocument.FriendInvitationId = friendshipInvitation.FriendshipInvitationId.Value;
					friendInvitationDocument.IsInviter = friendshipInvitation.IsInviter.Value;
					friendInvitationDocument.IsTrusted = friendshipInvitation.IsTrusted.Value;
					friendInvitationDocument.DisplayName = friendshipInvitation.FriendDisplayName;
					FriendInvitationDocument doc2 = friendInvitationDocument;
					InsertFriendInvitation(doc2);
				}
			}
			if (response.Users != null)
			{
				foreach (User user3 in response.Users)
				{
					PersistUser(user3.UserId, user3.HashedUserId, user3.DisplayName, user3.FirstName, user3.Status);
				}
			}
			callback();
		}
	}
}
