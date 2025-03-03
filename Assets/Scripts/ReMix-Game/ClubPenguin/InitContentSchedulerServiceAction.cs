using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitContentSchedulerServiceAction : InitActionComponent
	{
		public int PenguinStandardTimeOffsetHours = -8;

		public ScheduledEventDateDefinitionKey SupportWindow;

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
			Dictionary<int, ScheduledEventDateDefinition> events = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
			ScheduledEventDateDefinition def = null;
			events.TryGetValue(SupportWindow.Id, out def);
			ContentSchedulerService service = new ContentSchedulerService(scheduleAssetRequest.Asset.Map.Keys, PenguinStandardTimeOffsetHours, def);
			Service.Set(service);
			bool offlineMode = service.HasSupportEndded();
			string offline_mode = CommandLineArgs.GetValueForKey("offline_mode");
			if (!string.IsNullOrEmpty(offline_mode))
			{
				offlineMode = (offline_mode.ToLower().Trim() == "true");
			}
			Service.Get<GameSettings>().SetOfflineMode(offlineMode);
		}
	}
}
