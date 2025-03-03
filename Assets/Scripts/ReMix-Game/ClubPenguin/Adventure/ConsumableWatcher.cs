using ClubPenguin.Core;
using ClubPenguin.Props;
using ClubPenguin.Tags;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/ConsumableUse")]
	public class ConsumableWatcher : TaskWatcher
	{
		public enum ConsumableEvent
		{
			START,
			COMPLETE
		}

		public PropDefinition[] Props = new PropDefinition[0];

		public TagMatcher Tags;

		public ConsumableEvent Event = ConsumableEvent.COMPLETE;

		private PropService propService;

		private PropUser propUser;

		public override void OnActivate()
		{
			base.OnActivate();
			if (SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject != null)
			{
				propUser = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
				if (Event == ConsumableEvent.COMPLETE)
				{
					propUser.EPropRemoved += onPropUseCompleted;
				}
				else
				{
					onPropUseCompleted(propUser.Prop);
				}
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
		}

		public void OnDestroy()
		{
			if (propUser != null)
			{
				propUser.EPropUseCompleted -= onPropUseCompleted;
			}
		}

		private void onPropUseCompleted(Prop prop)
		{
			bool flag = false;
			if (Props != null)
			{
				for (int i = 0; i < Props.Length; i++)
				{
					if (prop.PropId == Props[i].GetNameOnServer())
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				taskIncrement();
			}
		}

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("event", Event);
			List<string> list = new List<string>();
			PropDefinition[] props = Props;
			foreach (PropDefinition propDefinition in props)
			{
				list.Add(propDefinition.GetNameOnServer());
			}
			dictionary.Add("consumables", list);
			dictionary.Add("tags", Tags.GetExportParameters());
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "consumable";
		}
	}
}
