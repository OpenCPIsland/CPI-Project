using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	public class InitInactivityServiceAction : InitActionComponent
	{
		public const string InactivityServiceEnabledPlayerPrefsKey = "inactivity_service_enabled";

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		public int InactivityTimeoutSeconds;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			InactivityService inactivityService = Service.Get<GameObject>().AddComponent<InactivityService>();
			inactivityService.InactivityTimeoutSeconds = InactivityTimeoutSeconds;
			inactivityService.SetTrackingEnabled(PlayerPrefs.GetInt(GetPlatformKey(InactivityServiceEnabledPlayerPrefsKey), 1) == 1);
			Service.Set(inactivityService);
			yield break;
		}
	}
}
