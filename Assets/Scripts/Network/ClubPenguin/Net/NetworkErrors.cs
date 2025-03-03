using ClubPenguin.Net.Domain;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class NetworkErrors
	{
		public struct GeneralError
		{
			public readonly ErrorResponse Error;

			public GeneralError(ErrorResponse error)
			{
				Error = error;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NoConnectionError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NotEnoughResourcesError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct InvalidSubscriptionError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SecurityAccessError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PlayerNotFoundError
		{
		}

		public struct InputBadRequestError
		{
			public readonly ErrorResponse Error;

			public InputBadRequestError(ErrorResponse error)
			{
				Error = error;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct GeneralResourceNotFoundError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct GeneralResourceNoLongerAvailableError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SystemInternalErrorError
		{
		}
	}
}
