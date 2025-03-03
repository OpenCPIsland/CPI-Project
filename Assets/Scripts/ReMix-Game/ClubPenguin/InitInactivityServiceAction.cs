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
			Service.Set(inactivityService);
			yield break;
		}
	}
}
