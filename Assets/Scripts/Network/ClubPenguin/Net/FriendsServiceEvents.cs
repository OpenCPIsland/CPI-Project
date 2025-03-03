using Disney.Kelowna.Common.DataModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Net
{
	public static class FriendsServiceEvents
	{
		public struct FindUserSent
		{
			public readonly bool Success;

			public FindUserSent(bool success)
			{
				Success = success;
			}
		}

		public struct SendFriendInvitationSent
		{
			public readonly bool Success;

			public readonly string FriendName;

			public SendFriendInvitationSent(bool success, string friendName)
			{
				Success = success;
				FriendName = friendName;
			}
		}

		public struct AcceptFriendInvitationSent
		{
			public readonly bool Success;

			public readonly string FriendName;

			public AcceptFriendInvitationSent(bool success, string friendName)
			{
				Success = success;
				FriendName = friendName;
			}
		}

		public struct RejectFriendInvitationSent
		{
			public readonly bool Success;

			public readonly string FriendName;

			public RejectFriendInvitationSent(bool success, string friendName)
			{
				Success = success;
				FriendName = friendName;
			}
		}

		public struct UnfriendSent
		{
			public readonly bool Success;

			public readonly string FriendName;

			public UnfriendSent(bool success, string friendName)
			{
				Success = success;
				FriendName = friendName;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FriendsServiceReady
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FriendsServiceInitialized
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FriendsServiceCleared
		{
		}

		public struct FriendsListUpdated
		{
			public readonly List<DataEntityHandle> FriendsList;

			public FriendsListUpdated(List<DataEntityHandle> friendsList)
			{
				FriendsList = friendsList;
			}
		}

		public struct IncomingInvitationsListUpdated
		{
			public readonly List<DataEntityHandle> IncomingInvitationsList;

			public IncomingInvitationsListUpdated(List<DataEntityHandle> incomingInvitationsList)
			{
				IncomingInvitationsList = incomingInvitationsList;
			}
		}

		public struct OutgoingInvitationsListUpdated
		{
			public readonly List<DataEntityHandle> OutgoingInvitationsList;

			public OutgoingInvitationsListUpdated(List<DataEntityHandle> outgoingInvitationsList)
			{
				OutgoingInvitationsList = outgoingInvitationsList;
			}
		}

		public struct FriendLocationInRoomReceived
		{
			public readonly Vector3 Location;

			public FriendLocationInRoomReceived(Vector3 location)
			{
				Location = location;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FriendNotInRoom
		{
		}
	}
}
