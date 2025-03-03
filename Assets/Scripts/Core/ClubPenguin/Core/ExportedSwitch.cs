using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class ExportedSwitch
	{
		public string type;

		public string path;

		public bool latch;

		public object parameters;

		public ExportedSwitch[] children;

		public static ExportedSwitch Create(Switch s)
		{
			ExportedSwitch exportedSwitch = new ExportedSwitch();
			exportedSwitch.type = s.GetSwitchType();
			exportedSwitch.path = s.gameObject.GetPath();
			exportedSwitch.latch = s.Latch;
			exportedSwitch.parameters = s.GetSwitchParameters();
			List<ExportedSwitch> list = new List<ExportedSwitch>();
			for (int i = 0; i < s.transform.childCount; i++)
			{
				Switch component = s.transform.GetChild(i).GetComponent<Switch>();
				if (component != null)
				{
					list.Add(Create(component));
				}
			}
			exportedSwitch.children = list.ToArray();
			return exportedSwitch;
		}
	}
}
