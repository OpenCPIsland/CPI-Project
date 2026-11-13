using System.Runtime.InteropServices;

namespace ClubPenguin
{
	public static class AccountSystemEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AccountSystemEnded
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AccountSystemDestroyed
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AccountSystemCreated
		{
		}
	}
}
