using ClubPenguin.Core;
using ClubPenguin.MiniGames.Fishing;
using ClubPenguin.Net;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Fishing")]
	public class FishingSuccessWatcher : TaskWatcher
	{
		[Tooltip("Matches any fishing reward in the list, or every reward if the list is empty")]
		public LootTableRewardDefinition[] loot;

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<MinigameServiceEvents.FishCaught>(onFishCaught);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<MinigameServiceEvents.FishCaught>(onFishCaught);
		}

		private bool onFishCaught(MinigameServiceEvents.FishCaught evt)
		{
			if (matches(evt.WinningRewardName))
			{
				taskIncrement();
			}
			return false;
		}

		private bool matches(string rewardName)
		{
			if (loot == null || loot.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < loot.Length; i++)
			{
				if (loot[i] != null && loot[i].Id == rewardName)
				{
					return true;
				}
			}
			return false;
		}

		public override object GetExportParameters()
		{
			List<string> list = new List<string>();
			LootTableRewardDefinition[] array = loot;
			foreach (LootTableRewardDefinition lootTableRewardDefinition in array)
			{
				list.Add(lootTableRewardDefinition.Id);
			}
			return list;
		}

		public override string GetWatcherType()
		{
			return "fish";
		}
	}
}
