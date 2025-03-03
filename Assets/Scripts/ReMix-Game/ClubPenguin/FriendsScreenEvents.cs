using Disney.Kelowna.Common.DataModel;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin
{
	public static class FriendsScreenEvents
	{
		public struct AvatarImageReady
		{
			public readonly DataEntityHandle Handle;

			public readonly Texture2D Icon;

			public AvatarImageReady(DataEntityHandle handle, Texture2D icon)
			{
				Handle = handle;
				Icon = icon;
			}
		}

		public struct SendFindUser
		{
			public readonly string DisplayName;

			public SendFindUser(string displayName)
			{
				DisplayName = displayName;
			}
		}

		public struct SearchStringUpdated
		{
			public readonly string SearchString;

			public SearchStringUpdated(string searchString)
			{
				SearchString = searchString;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SearchClicked
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SearchFriend
		{
		}
	}
}
