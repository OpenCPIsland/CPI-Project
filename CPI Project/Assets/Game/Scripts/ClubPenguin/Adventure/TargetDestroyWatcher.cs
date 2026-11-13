using ClubPenguin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Targets/TargetDestroyed")]
	public class TargetDestroyWatcher : TaskWatcher
	{
		[Tooltip("Leave empty to match all targets")]
		public TagDefinition[] MatchingTagDefinitions;

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<string> list = new List<string>();
			TagDefinition[] matchingTagDefinitions = MatchingTagDefinitions;
			foreach (TagDefinition tagDefinition in matchingTagDefinitions)
			{
				list.Add(tagDefinition.Tag);
			}
			dictionary.Add("matchingTags", list);
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "targetdestroyed";
		}
	}
}
