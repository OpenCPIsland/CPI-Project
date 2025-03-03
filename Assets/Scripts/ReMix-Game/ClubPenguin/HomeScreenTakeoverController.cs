using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class HomeScreenTakeoverController : MonoBehaviour
	{
		public PrefabContentKey DefaultHomeScreen;

		public Transform TargetParent;

		private void Awake()
		{
			Service.Get<LoadingController>().AddLoadingSystem(this);
			CoroutineRunner.Start(Service.Get<GameData>().LoadDataForDefinitions(InitGameDataAction.DefinitionTypesToLoadAfterBoot, OnDefinitionsLoaded), this, "Service.Get<GameData>().LoadDataForDefinitions(InitGameDataAction.DefinitionTypesToLoadAfterBoot, OnDefinitionsLoaded)");
		}

		private void OnDefinitionsLoaded()
		{
			PrefabContentKey key = DefaultHomeScreen;
			Dictionary<string, HomeScreenTakeoverDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, HomeScreenTakeoverDefinition>>();
			foreach (KeyValuePair<string, HomeScreenTakeoverDefinition> item in dictionary)
			{
				ScheduledEventDateDefinition scheduledEventDatedDef = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[item.Value.DateDefinitionKey.Id];
				if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(scheduledEventDatedDef))
				{
					if (Content.ContainsKey(item.Value.TakeOverPrefabContentKey))
					{
						key = item.Value.TakeOverPrefabContentKey;
						break;
					}
					Log.LogErrorFormatted(this, "The home screen prefab {0} for the scheduled event {1} is was not found, falling back to another", item.Value.TakeOverPrefabContentKey, item.Value.name);
				}
			}
			Content.LoadAsync(onHomeScreenPrefabLoaded, key);
		}

		private void onHomeScreenPrefabLoaded(string path, GameObject homeScreenPrefab)
		{
			Object.Instantiate(homeScreenPrefab, TargetParent);
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
		}
	}
}
