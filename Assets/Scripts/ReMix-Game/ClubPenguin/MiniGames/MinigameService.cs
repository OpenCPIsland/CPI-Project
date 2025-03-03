using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.MiniGames.Fishing;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames
{
	public class MinigameService
	{
		public Dictionary<string, LootTableRewardDefinition> Loot = new Dictionary<string, LootTableRewardDefinition>();

		private SignedResponse<FishingResult> fishingResultFromCast;

		public MinigameService(Manifest lootManifest)
		{
			ScriptableObject[] assets = lootManifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				LootTableRewardDefinition lootTableRewardDefinition = (LootTableRewardDefinition)scriptableObject;
				Loot[lootTableRewardDefinition.Id] = lootTableRewardDefinition;
			}
			Service.Get<EventDispatcher>().AddListener<MinigameServiceEvents.FishingResultRecieved>(onFishingResultRecieved);
		}

		public IEnumerator loadRewardManifest()
		{
			ManifestContentKey lootRewardsManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(LootTableRewardDefinition));
			AssetRequest<Manifest> assetRequest = Content.LoadAsync(lootRewardsManifestContentKey);
			yield return assetRequest;
			Manifest lootManifest = assetRequest.Asset;
			ScriptableObject[] assets = lootManifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				LootTableRewardDefinition lootTableRewardDefinition = (LootTableRewardDefinition)scriptableObject;
				Loot[lootTableRewardDefinition.Id] = lootTableRewardDefinition;
			}
		}

		public LootTableRewardDefinition GetLootRewardDefinition(string prizeName)
		{
			if (!Loot.ContainsKey(prizeName))
			{
				using (Dictionary<string, LootTableRewardDefinition>.KeyCollection.Enumerator enumerator = Loot.Keys.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						return Loot[current];
					}
				}
			}
			return Loot[prizeName];
		}

		public void CastFishingRod(ICastFishingRodErrorHandler errorHandler)
		{
			fishingResultFromCast = null;
			Service.Get<INetworkServicesManager>().MinigameService.CastFishingRod(errorHandler);
		}

		public void CatchFish(string winningRewardName)
		{
			if (fishingResultFromCast != null)
			{
				Service.Get<INetworkServicesManager>().MinigameService.CatchFish(fishingResultFromCast, winningRewardName);
				fishingResultFromCast = null;
				return;
			}
			throw new InvalidOperationException("There is nothing to catch!");
		}

		private bool onFishingResultRecieved(MinigameServiceEvents.FishingResultRecieved evt)
		{
			fishingResultFromCast = evt.Result;
			return false;
		}
	}
}
