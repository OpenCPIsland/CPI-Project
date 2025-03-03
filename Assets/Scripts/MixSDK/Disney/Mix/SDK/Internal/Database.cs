using DeviceDB;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Disney.Mix.SDK.Internal
{
	public class Database : IDatabase
	{
		private const string KeyValuesDocumentCollectionName = "KeyValues";

		private const string SessionGroupIdKey = "SessionGroupIdKey";

		private const string RsaParametersKey = "RsaParametersKey";

		private const string ServerTimeOffsetMillisKey = "ServerTimeOffsetMillis";

		private const string GuestControllerApiKeyKey = "GuestControllerApiKey";

		private readonly byte[] encryptionKey;

		private readonly IRandom random;

		private readonly IEpochTime epochTime;

		private readonly IDatabaseDirectoryCreator directoryCreator;

		private readonly IDocumentCollectionFactory documentCollectionFactory;

		private readonly DatabaseCorruptionHandler databaseCorruptionHandler;

		public Database(byte[] encryptionKey, IRandom random, IEpochTime epochTime, IDatabaseDirectoryCreator directoryCreator, IDocumentCollectionFactory documentCollectionFactory, DatabaseCorruptionHandler databaseCorruptionHandler)
		{
			this.encryptionKey = encryptionKey;
			this.random = random;
			this.epochTime = epochTime;
			this.directoryCreator = directoryCreator;
			this.documentCollectionFactory = documentCollectionFactory;
			this.databaseCorruptionHandler = databaseCorruptionHandler;
		}

		public long GetSessionGroupId()
		{
			string value = GetValue("SessionGroupIdKey");
			if (value == null)
			{
				int num = random.Next(2147483646) + 1;
				SetValue("SessionGroupIdKey", num.ToString());
				return num;
			}
			return long.Parse(value);
		}

		public RSAParameters? GetRsaParameters()
		{
			string value = GetValue("RsaParametersKey");
			return (value == null) ? null : JsonParser.NullableFromJson<RSAParameters>(value);
		}

		public void SetRsaParameters(RSAParameters rsaParameters)
		{
			string value = JsonParser.ToJson(rsaParameters);
			SetValue("RsaParametersKey", value);
		}

		public long? GetServerTimeOffsetMillis()
		{
			string value = GetValue("ServerTimeOffsetMillis");
			return (value == null) ? null : new long?(long.Parse(value));
		}

		public void SetServerTimeOffsetMillis(long offsetMillis)
		{
			SetValue("ServerTimeOffsetMillis", offsetMillis.ToString());
		}

		public void ClearServerTimeOffsetMillis()
		{
			ClearValue("ServerTimeOffsetMillis");
		}

		public string GetGuestControllerApiKey()
		{
			return GetValue("GuestControllerApiKey");
		}

		public void SetGuestControllerApiKey(string apiKey)
		{
			SetValue("GuestControllerApiKey", apiKey);
		}

		public void ClearGuestControllerApiKey()
		{
			ClearValue("GuestControllerApiKey");
		}

		private string GetValue(string key)
		{
			string value = null;
			CreateKeyValues(delegate(IDocumentCollection<KeyValueDocument> keyValues)
			{
				uint[] array = keyValues.FindDocumentIdsEqual(KeyValueDocument.KeyFieldName, key).ToArray();
				if (array.Length > 0)
				{
					KeyValueDocument keyValueDocument = keyValues.Find(array[0]);
					value = keyValueDocument.Value;
				}
			});
			return value;
		}

		private void SetValue(string key, string value)
		{
			CreateKeyValues(delegate(IDocumentCollection<KeyValueDocument> keyValues)
			{
				uint[] array = keyValues.FindDocumentIdsEqual(KeyValueDocument.KeyFieldName, key).ToArray();
				KeyValueDocument keyValueDocument = new KeyValueDocument
				{
					Key = key,
					Value = value
				};
				if (array.Length == 0)
				{
					keyValues.Insert(keyValueDocument);
				}
				else
				{
					keyValueDocument.Id = array[0];
					keyValues.Update(keyValueDocument);
				}
			});
		}

		private void ClearValue(string key)
		{
			CreateKeyValues(delegate(IDocumentCollection<KeyValueDocument> keyValues)
			{
				uint[] array = keyValues.FindDocumentIdsEqual(KeyValueDocument.KeyFieldName, key).ToArray();
				uint[] array2 = array;
				foreach (uint documentId in array2)
				{
					keyValues.Delete(documentId);
				}
			});
		}

		private void CreateKeyValues(Action<IDocumentCollection<KeyValueDocument>> operation)
		{
			string dirPath = directoryCreator.CreateSdkDirectory("KeyValues");
			try
			{
				using (IDocumentCollection<KeyValueDocument> documentCollection = documentCollectionFactory.CreateHighSecurityFileSystemCollection<KeyValueDocument>(dirPath, encryptionKey))
				{
					databaseCorruptionHandler.Add(documentCollection);
					operation(documentCollection);
					databaseCorruptionHandler.Remove(documentCollection);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		public void StoreSession(string swid, string accessToken, string refreshToken, string displayName, string firstName, string etag, string ageBand, string proposedDisplayName, string proposedDisplayNameStatus, string accountStatus, bool updateLastProfileRefreshTime, string countryCode)
		{
			CreateSessions(delegate(IDocumentCollection<SessionDocument> sessions)
			{
				uint now = epochTime.Seconds;
				uint[] array = sessions.FindDocumentIdsEqual(SessionDocument.SwidFieldName, swid).ToArray();
				Action<SessionDocument, bool> action = delegate(SessionDocument doc, bool newDoc)
				{
					doc.GuestControllerAccessToken = accessToken;
					doc.GuestControllerRefreshToken = refreshToken;
					doc.Swid = swid;
					doc.LastSessionUpdateTime = now;
					if (newDoc)
					{
						doc.LastNotificationTime = long.MinValue;
					}
					doc.DisplayNameText = displayName;
					doc.FirstName = firstName;
					if (etag != null)
					{
						doc.GuestControllerEtag = etag;
					}
					doc.AgeBand = ageBand;
					doc.LoggedOut = false;
					doc.ProposedDisplayName = proposedDisplayName;
					doc.ProposedDisplayNameStatus = proposedDisplayNameStatus;
					doc.AccountStatus = accountStatus;
					doc.LatestNotificationSequenceNumber = 0L;
					doc.CountryCode = countryCode;
					doc.ProtocolVersion = 3;
				};
				if (array.Length > 0)
				{
					SessionDocument sessionDocument = sessions.Find(array[0]);
					action(sessionDocument, false);
					if (updateLastProfileRefreshTime)
					{
						sessionDocument.LastProfileRefreshTime = epochTime.Seconds;
					}
					sessions.Update(sessionDocument);
				}
				else
				{
					SessionDocument sessionDocument = new SessionDocument
					{
						LastProfileRefreshTime = epochTime.Seconds
					};
					action(sessionDocument, true);
					sessions.Insert(sessionDocument);
				}
			});
		}

		public void UpdateGuestControllerToken(Token token, string etag)
		{
			UpdateSessionDocument(token.swid, delegate(SessionDocument doc)
			{
				doc.GuestControllerAccessToken = token.access_token;
				doc.GuestControllerRefreshToken = token.refresh_token;
				if (etag != null)
				{
					doc.GuestControllerEtag = etag;
				}
			});
		}

		public SessionDocument GetLastLoggedInSessionDocument()
		{
			SessionDocument doc = null;
			CreateSessions(delegate(IDocumentCollection<SessionDocument> sessions)
			{
				doc = (from d in sessions
					where !d.LoggedOut
					orderby d.LastSessionUpdateTime descending
					select d).FirstOrDefault();
			});
			return doc;
		}

		public SessionDocument GetSessionDocument(string swid)
		{
			SessionDocument doc = null;
			CreateSessions(delegate(IDocumentCollection<SessionDocument> sessions)
			{
				doc = GetSessionDocument(swid, sessions);
			});
			return doc;
		}

		public void UpdateSessionDocument(string swid, Action<SessionDocument> updateCallback)
		{
			CreateSessions(delegate(IDocumentCollection<SessionDocument> sessions)
			{
				SessionDocument sessionDocument = GetSessionDocument(swid, sessions);
				if (sessionDocument != null)
				{
					updateCallback(sessionDocument);
					sessionDocument.ProtocolVersion = 3;
					sessions.Update(sessionDocument);
				}
			});
		}

		public void LogOutSession(string swid)
		{
			UpdateSessionDocument(swid, delegate(SessionDocument doc)
			{
				doc.LoggedOut = true;
				doc.PushNotificationToken = null;
				doc.PushNotificationTokenType = null;
				doc.VisiblePushNotificationsEnabled = false;
				doc.ProvisionId = null;
			});
		}

		public void DeleteSession(string swid)
		{
			CreateSessions(delegate(IDocumentCollection<SessionDocument> sessions)
			{
				uint? sessionDocumentId = GetSessionDocumentId(swid, sessions);
				if (sessionDocumentId.HasValue)
				{
					sessions.Delete(sessionDocumentId.Value);
				}
			});
		}

		private void CreateSessions(Action<IDocumentCollection<SessionDocument>> operation)
		{
			string dirPath = directoryCreator.CreateSdkDirectory("Sessions");
			try
			{
				using (IDocumentCollection<SessionDocument> documentCollection = documentCollectionFactory.CreateHighSecurityFileSystemCollection<SessionDocument>(dirPath, encryptionKey))
				{
					databaseCorruptionHandler.Add(documentCollection);
					operation(documentCollection);
					databaseCorruptionHandler.Remove(documentCollection);
				}
			}
			catch (CorruptionException ex)
			{
				databaseCorruptionHandler.HandleCorruption(ex);
				throw;
			}
		}

		private static SessionDocument GetSessionDocument(string swid, IDocumentCollection<SessionDocument> sessions)
		{
			uint? sessionDocumentId = GetSessionDocumentId(swid, sessions);
			return sessionDocumentId.HasValue ? sessions.Find(sessionDocumentId.Value) : null;
		}

		private static uint? GetSessionDocumentId(string swid, IDocumentCollection<SessionDocument> sessions)
		{
			uint[] array = sessions.FindDocumentIdsEqual(SessionDocument.SwidFieldName, swid).ToArray();
			return (array.Length == 0) ? null : new uint?(array[0]);
		}
	}
}
