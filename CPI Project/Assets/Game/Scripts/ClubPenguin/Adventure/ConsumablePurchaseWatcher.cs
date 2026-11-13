using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Props;
using ClubPenguin.Tags;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
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

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<ConsumableServiceEvents.ConsumablePurchased>(onConsumablePurchased);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<ConsumableServiceEvents.ConsumablePurchased>(onConsumablePurchased);
		}

		private bool onConsumablePurchased(ConsumableServiceEvents.ConsumablePurchased evt)
		{
			if (matches(evt.Type))
			{
				int num = Mathf.Max(1, evt.Count);
				for (int i = 0; i < num; i++)
				{
					taskIncrement();
				}
			}
			return false;
		}

		private bool matches(string type)
		{
			if (props != null && props.Length > 0)
			{
				for (int i = 0; i < props.Length; i++)
				{
					if (props[i] != null && props[i].isDefinition(type))
					{
						return true;
					}
				}
			}
			if (hasAnyFilter(Tags))
			{
				PropDefinition value;
				if (Service.Get<PropService>().Props.TryGetValue(type, out value))
				{
					return Tags.isMatch(new TagsArray[1]
					{
						new TagsArray(value.Tags)
					});
				}
				return false;
			}
			return props == null || props.Length == 0;
		}

		private static bool hasAnyFilter(BaseTagMatcher matcher)
		{
			if (matcher == null)
			{
				return false;
			}
			if ((matcher.Tags != null && matcher.Tags.Length > 0) || (matcher.Categories != null && matcher.Categories.Length > 0))
			{
				return true;
			}
			BaseRecursiveTagMatcher baseRecursiveTagMatcher = matcher as BaseRecursiveTagMatcher;
			if (baseRecursiveTagMatcher != null && baseRecursiveTagMatcher.Matchers != null)
			{
				for (int i = 0; i < baseRecursiveTagMatcher.Matchers.Length; i++)
				{
					if (hasAnyFilter(baseRecursiveTagMatcher.Matchers[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

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
