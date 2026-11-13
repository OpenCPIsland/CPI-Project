using ClubPenguin.SceneLayoutSync;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDataModelServicesAction))]
	public class InitSceneLayoutSyncServiceAction : InitActionComponent
	{
		[Range(5f, 120f)]
		public float SyncPeriodInSeconds = 30f;

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
			Service.Set(new SceneLayoutSyncService(SyncPeriodInSeconds));
			Service.Set(new SceneLayoutDataManager());
			yield break;
		}
	}
}
