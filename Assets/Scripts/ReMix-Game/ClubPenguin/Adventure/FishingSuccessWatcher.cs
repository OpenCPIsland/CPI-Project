using ClubPenguin.Core;
using ClubPenguin.MiniGames.Fishing;
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
