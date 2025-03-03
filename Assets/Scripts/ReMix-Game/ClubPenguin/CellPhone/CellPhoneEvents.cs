using System.Runtime.InteropServices;

namespace ClubPenguin.CellPhone
{
	public static class CellPhoneEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CellPhoneOpened
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CellPhoneClosed
		{
		}

		public struct ChangeCellPhoneScreen
		{
			public CellPhoneAppBehaviour Behaviour;

			public string BehaviourParam;

			public string AppName;

			public string LoadingScreenOverride;

			public int AppletSceneSystemMemoryThreshold;

			public ChangeCellPhoneScreen(CellPhoneAppBehaviour behaviour, string behaviourParam, string appName, string loadingScreenOverride, int appletSceneSystemMemoryThreshold)
			{
				Behaviour = behaviour;
				BehaviourParam = behaviourParam;
				AppName = appName;
				LoadingScreenOverride = loadingScreenOverride;
				AppletSceneSystemMemoryThreshold = appletSceneSystemMemoryThreshold;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ReturnToHomeScreen
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideLoadingScreen
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct FriendsLoadComplete
		{
		}
	}
}
