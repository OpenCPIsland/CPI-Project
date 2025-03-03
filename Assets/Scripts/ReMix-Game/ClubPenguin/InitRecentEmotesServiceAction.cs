using ClubPenguin.Chat;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitRecentEmotesServiceAction : InitActionComponent
	{
		public int RecentEmotesMaxCount;

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
			RecentEmotesService recentEmotesService = new RecentEmotesService();
			recentEmotesService.RecentEmotesMaxCount = RecentEmotesMaxCount;
			Service.Set(recentEmotesService);
			yield break;
		}
	}
}
