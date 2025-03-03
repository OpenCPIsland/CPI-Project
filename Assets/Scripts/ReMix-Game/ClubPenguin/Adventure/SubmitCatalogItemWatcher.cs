using ClubPenguin.Core;
using ClubPenguin.Tags;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/SubmitCatalogItem")]
	public class SubmitCatalogItemWatcher : TaskWatcher
	{
		public OutfitTagMatcher TagMatcher;

		[Tooltip("Leave empty to match all themes")]
		public CatalogThemeDefinition[] MatchingThemes;

		public override object GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("matchingThemeChallenges", MatchingThemes);
			dictionary.Add("matcher", TagMatcher.GetExportParameters());
			return dictionary;
		}

		public override string GetWatcherType()
		{
			return "submitCatalogItem";
		}
	}
}
