using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Disney.Mix.SDK.Internal
{
	public static class PushNotificationReceiver
	{
		private static readonly Encoding stringEncoding = Encoding.UTF8;

		public static IInternalPushNotification Receive(AbstractLogger logger, IEncryptor encryptor, IDatabase database, string swid, IDictionary notification)
		{
			SessionDocument sessionDocument = database.GetSessionDocument(swid);
			byte[] currentSymmetricEncryptionKey = sessionDocument.CurrentSymmetricEncryptionKey;
			string optionalString = GetOptionalString(notification, "payload");
			IDictionary dictionary;
			if (optionalString == null)
			{
				dictionary = notification;
			}
			else
			{
				byte[] payloadEncryptedBytes;
				try
				{
					payloadEncryptedBytes = Convert.FromBase64String(optionalString);
				}
				catch (Exception arg)
				{
					throw new ArgumentException("Couldn't deserialize push notification: " + arg);
				}
				try
				{
					dictionary = DecryptPayload(encryptor, currentSymmetricEncryptionKey, payloadEncryptedBytes);
				}
				catch (Exception arg)
				{
					byte[] previousSymmetricEncryptionKey = sessionDocument.PreviousSymmetricEncryptionKey;
					if (previousSymmetricEncryptionKey == null)
					{
						throw new ArgumentException("Couldn't decrypt push notification: " + arg);
					}
					try
					{
						dictionary = DecryptPayload(encryptor, previousSymmetricEncryptionKey, payloadEncryptedBytes);
					}
					catch (Exception ex)
					{
						throw new ArgumentException(string.Concat("Couldn't decrypt push notification: ", arg, "\n", ex));
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder("Received push notification with payload:\n");
			foreach (DictionaryEntry item in dictionary)
			{
				stringBuilder.Append(item.Key);
				stringBuilder.Append(" = ");
				stringBuilder.Append(item.Value);
				stringBuilder.Append('\n');
			}
			logger.Debug(stringBuilder.ToString());
			string optionalString2 = GetOptionalString(dictionary, "type");
			if (optionalString2 == null)
			{
				throw new ArgumentException("Push notification doesn't have a type");
			}
			string optionalString3 = GetOptionalString(dictionary, "notifications_available");
			bool notificationsAvailable = optionalString3 == null || optionalString3 == "true";
			switch (optionalString2)
			{
			case "FRIENDSHIP_INVITATION_MESSAGE":
			{
				string optionalString5 = GetOptionalString(dictionary, "friendshipInvitationId");
				return new FriendshipInvitationReceivedPushNotification(notificationsAvailable, optionalString5);
			}
			case "FRIENDSHIP_MESSAGE":
			{
				string optionalString4 = GetOptionalString(dictionary, "friendId");
				return new FriendshipAddedPushNotification(notificationsAvailable, optionalString4);
			}
			case "BROADCAST":
				return new BroadcastPushNotification(notificationsAvailable);
			case "ADD_ALERT":
				return new AlertAddedPushNotification(notificationsAvailable);
			case "CLEAR_ALERT":
				return new AlertsClearedPushNotification(notificationsAvailable);
			case "REMOVE_FRIENDSHIP_INVITATION":
				return new FriendshipInvitationRemovedPushNotification(notificationsAvailable);
			case "REMOVE_FRIENDSHIP_TRUST":
				return new UntrustedPushNotification(notificationsAvailable);
			default:
				return new GenericPushNotification(notificationsAvailable);
			}
		}

		private static IDictionary DecryptPayload(IEncryptor encryptor, byte[] key, byte[] payloadEncryptedBytes)
		{
			byte[] bytes = encryptor.Decrypt(payloadEncryptedBytes, key);
			string @string = stringEncoding.GetString(bytes);
			return JsonParser.FromJson<Dictionary<string, object>>(@string);
		}

		private static string GetOptionalString(IDictionary dict, string name)
		{
			if (!dict.Contains(name))
			{
				return null;
			}
			object obj = dict[name];
			if (!(obj is string))
			{
				return null;
			}
			return (string)obj;
		}
	}
}
