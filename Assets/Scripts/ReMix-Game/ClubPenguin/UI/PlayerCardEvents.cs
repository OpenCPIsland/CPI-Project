using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public static class PlayerCardEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DismissPlayerCard
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SendFriendInvitation
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AcceptFriendInvitation
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ReportPlayer
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct JoinPlayer
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct UnfriendPlayer
		{
		}

		public struct SetEnablePlayerCard
		{
			public readonly bool Enabled;

			public SetEnablePlayerCard(bool enable)
			{
				Enabled = enable;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PlayerCardClosed
		{
		}
	}
}
