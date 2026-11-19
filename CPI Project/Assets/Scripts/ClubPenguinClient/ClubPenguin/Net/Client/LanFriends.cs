using ClubPenguin.Net.Offline;
using System;

namespace ClubPenguin.Net.Client
{
	public interface ILanFriendsTransport
	{
		void Find(string name, Action<string> done);

		void Invite(string name, Action<LanFriendsData.Entry?, string> done);

		void Accept(long id, Action<bool> done);

		void Reject(long id, Action<bool> done);

		void Unfriend(string swid, Action<bool> done);
	}

	public static class LanFriends
	{
		public static ILanFriendsTransport Transport;

		private static Action<string, LanFriendsData.Entry> listener;

		private static string listenerToken;

		public static void Listen(string token, Action<string, LanFriendsData.Entry> action)
		{
			listenerToken = token;
			listener = action;
		}

		public static bool Deliver(string token, string kind, LanFriendsData.Entry entry)
		{
			if (listener == null || listenerToken != token)
			{
				return false;
			}
			listener(kind, entry);
			return true;
		}

		public static LanFriendsData Load(string token)
		{
			LanFriendsData data = OfflineDatabase.Read<LanFriendsData>(token);
			if (data.Friends == null || data.Incoming == null || data.Outgoing == null)
			{
				LanFriendsData fixedData = default(LanFriendsData);
				fixedData.Init();
				if (data.Friends != null)
				{
					fixedData.Friends = data.Friends;
				}
				if (data.Incoming != null)
				{
					fixedData.Incoming = data.Incoming;
				}
				if (data.Outgoing != null)
				{
					fixedData.Outgoing = data.Outgoing;
				}
				data = fixedData;
			}
			return data;
		}

		public static void Save(LanFriendsData data, string token)
		{
			OfflineDatabase.Write(data, token);
		}
	}
}
