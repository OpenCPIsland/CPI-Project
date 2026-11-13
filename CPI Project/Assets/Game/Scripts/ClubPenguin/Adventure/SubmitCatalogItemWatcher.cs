using ClubPenguin.Core;
using ClubPenguin.Net;
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

		// The clothing designer is a scene of its own
		public override bool SurvivesSceneChange
		{
			get
			{
				return true;
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<CatalogServiceEvents.ItemSubmissionCompleteEvent>(onItemSubmitted);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<CatalogServiceEvents.ItemSubmissionCompleteEvent>(onItemSubmitted);
		}

		// Only an accepted submission raises this, a refusal comes back as an error
		private bool onItemSubmitted(CatalogServiceEvents.ItemSubmissionCompleteEvent evt)
		{
			taskIncrement();
			return false;
		}

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
