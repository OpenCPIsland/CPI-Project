using ClubPenguin.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public static class AllAccessHelper
	{
		public class AllAccessEventKeyGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				Dictionary<string, AllAccessEventDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, AllAccessEventDefinition>>();
				foreach (KeyValuePair<string, AllAccessEventDefinition> item in dictionary)
				{
					list.Add(new NamedToggleValueAttribute.NamedToggleValue(item.Value.Id, item.Value.Id));
				}
				return list;
			}
		}

		private const string SEEN_ALL_ACCESS_KEY = "SeenAllAccessFlow_";

		private const string SHOW_ALL_ACCESS_OVER_KEY = "SeenAllAccessOverFlow";

		public static bool HasSeenAllAccessFlow(string allAccessEventKey, string displayName)
		{
			string key = "SeenAllAccessFlow_" + allAccessEventKey + "_" + displayName;
			return PlayerPrefs.HasKey(key);
		}

		public static void SetHasSeenAllAccessFlow(string allAccessEventKey, string displayName)
		{
			string key = "SeenAllAccessFlow_" + allAccessEventKey + "_" + displayName;
			PlayerPrefs.SetString(key, "");
			PlayerPrefs.SetString("SeenAllAccessOverFlow", "");
		}

		public static bool ShowAccessEndedFlow()
		{
			return PlayerPrefs.HasKey("SeenAllAccessOverFlow");
		}

		public static void SetHasSeenAllAccessEndedFlow()
		{
			PlayerPrefs.DeleteKey("SeenAllAccessOverFlow");
		}

		[Invokable("Tests.AllAccess.DoNotShowMeAccessEndedFlow")]
		public static void DoNotShowMeAccessEndedFlow()
		{
			SetHasSeenAllAccessEndedFlow();
		}

		[Invokable("Tests.AllAccess.ShowMeAccessEndedFlow")]
		public static void ShowMeAccessEndedFlow()
		{
			PlayerPrefs.SetString("SeenAllAccessOverFlow", "");
		}

		[Invokable("Tests.AllAccess.DoNotShowMeAllAccessFlowForKey")]
		public static void DoNotShowMeAllAccessFlowForKey([NamedToggleValue(typeof(AllAccessEventKeyGenerator), 0u)] string allAccessEventKey, string displayName)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = getLocalPlayerDisplayName();
			}
			SetHasSeenAllAccessFlow(allAccessEventKey, displayName);
		}

		[Invokable("Tests.AllAccess.ShowMeAllAccessFlowForKey")]
		public static void ShowMeAllAccessFlowForKey([NamedToggleValue(typeof(AllAccessEventKeyGenerator), 0u)] string allAccessEventKey, string displayName)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = getLocalPlayerDisplayName();
			}
			string key = "SeenAllAccessFlow_" + allAccessEventKey + "_" + displayName;
			PlayerPrefs.DeleteKey(key);
		}

		private static string getLocalPlayerDisplayName()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				return component.DisplayName;
			}
			return null;
		}
	}
}
