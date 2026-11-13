using ClubPenguin.Igloo;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	public class InitRecentIglooDecorationsServiceAction : InitActionComponent
	{
		public int MaxCountRecentDecorations = 16;

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
			RecentDecorationsService recentDecorationsService = new RecentDecorationsService();
			recentDecorationsService.MaxCountRecentDecorations = MaxCountRecentDecorations;
			Service.Set(recentDecorationsService);
			yield break;
		}
	}
}
