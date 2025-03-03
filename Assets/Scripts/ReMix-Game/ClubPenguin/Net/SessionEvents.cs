using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class SessionEvents
	{
		public struct SessionStartedEvent
		{
			public readonly string AccessToken;

			public readonly string LocalPlayerSwid;

			public SessionStartedEvent(string accessToken, string localPlayerSwid)
			{
				AccessToken = accessToken;
				LocalPlayerSwid = localPlayerSwid;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionEndedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionTerminatedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionPausedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionPausingEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionResumedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionLogoutEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StartGameEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ContinueFTUEEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FTUENameObjectiveCompleteEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FTUENameObjectiveCancelledEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FTUENameObjectiveAlreadyDoneEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ReturnToRestorePurchases
		{
		}

		public struct AccessTokenUpdatedEvent
		{
			public readonly string AccessToken;

			public AccessTokenUpdatedEvent(string accessToken)
			{
				this = default(AccessTokenUpdatedEvent);
				AccessToken = accessToken;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AuthenticationLostEvent
		{
		}
	}
}
