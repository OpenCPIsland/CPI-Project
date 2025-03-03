using ClubPenguin.Core;
using ClubPenguin.Props;
using ClubPenguin.Tags;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Consumable Purchase")]
	public class ConsumablePurchaseWatcher : TaskWatcher
	{
		[Tooltip("Matches any consumable in the list, or every consumable if the list is empty")]
		public PropDefinition[] props = new PropDefinition[0];

		public TagMatcher Tags;

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<string> list = new List<string>();
			PropDefinition[] array = props;
			foreach (PropDefinition propDefinition in array)
			{
				if (propDefinition.PropType != 0)
				{
					Log.LogError(this, "Invalid consumable " + propDefinition.name);
				}
				else
				{
					list.Add(propDefinition.GetNameOnServer());
				}
			}
			dictionary.Add("consumables", list);
			dictionary.Add("tags", Tags.GetExportParameters());
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "consumablePurchase";
		}
	}
}
