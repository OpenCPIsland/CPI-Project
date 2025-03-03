using ClubPenguin.DailyChallenge;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitAdventureSystemAction))]
	public class InitDailyChallengesServiceAction : InitActionComponent
	{
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
			string dailyChallengesScheduleManifestPath = DailyChallengeService.GetDateManifestMapPath();
			AssetRequest<DatedManifestMap> scheduleAssetRequest = Content.LoadAsync<DatedManifestMap>(dailyChallengesScheduleManifestPath);
			yield return scheduleAssetRequest;
			Service.Set(new DailyChallengeService(scheduleAssetRequest.Asset));
		}
	}
}
