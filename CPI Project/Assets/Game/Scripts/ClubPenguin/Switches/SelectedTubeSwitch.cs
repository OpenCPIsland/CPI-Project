using ClubPenguin.Core;
using ClubPenguin.Tags;
using ClubPenguin.Tubes;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class SelectedTubeSwitch : Switch
	{
		[Tooltip("List of tubes that will enable this switch. If empty matches NOTHING")]
		public TubeDefinition[] Tubes = new TubeDefinition[0];

		public TagMatcher Tags;

		public override object GetSwitchParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<int> list = new List<int>();
			TubeDefinition[] tubes = Tubes;
			foreach (TubeDefinition tubeDefinition in tubes)
			{
				list.Add(tubeDefinition.Id);
			}
			dictionary.Add("tubes", list);
			dictionary.Add("tags", Tags.GetExportParameters());
			return dictionary;
		}

		public override string GetSwitchType()
		{
			return "selectedTube";
		}
	}
}
