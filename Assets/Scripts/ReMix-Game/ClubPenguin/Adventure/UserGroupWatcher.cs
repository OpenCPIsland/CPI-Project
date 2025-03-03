using ClubPenguin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/GroupUsers")]
	public class UserGroupWatcher : TaskWatcher
	{
		public enum Operators
		{
			OR,
			AND
		}

		[Serializable]
		public class MultiUserSwitch
		{
			public int MinUserCount;

			public string SwitchName;
		}

		public Operators Operator = Operators.AND;

		public MultiUserSwitch[] Switches;

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("operator", Operator);
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			MultiUserSwitch[] switches = Switches;
			foreach (MultiUserSwitch multiUserSwitch in switches)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("minUserCount", multiUserSwitch.MinUserCount);
				dictionary2.Add("switchDef", ExportedSwitch.Create(GameObject.Find(multiUserSwitch.SwitchName).GetComponent<Switch>()));
				list.Add(dictionary2);
			}
			dictionary.Add("switches", list);
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "room";
		}
	}
}
