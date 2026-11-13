using ClubPenguin.SledRacer;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SledRacerFinishPopup : BasicPopup
	{
		private const int BLUE_TINT_INDEX = 0;

		private const int RED_TINT_INDEX = 1;

		private PrefabContentKey RankEntryPrefabKey = new PrefabContentKey("Prefabs/RankPanel");

		private PrefabContentKey RankEntrySelectedPrefabKey = new PrefabContentKey("Prefabs/RankPanelSelected");

		public GameObject RankEntryParent;

		public SpriteSelector[] SpriteSelectors;

		public TintSelector[] TintSelectors;

		private GameObject rankEntryPrefab;

		private GameObject rankEntrySelectedPrefab;

		private RaceResults raceResults;

		private long[] rankTimes;

		public void Initialize(RaceResults raceResults, long[] rankTimes)
		{
			this.raceResults = raceResults;
			this.rankTimes = rankTimes;
			CoroutineRunner.Start(loadRankEntryPrefabs(), this, "LoadRankEntryPrefabs");
			int index = 0;
			if (raceResults.trackId == "red")
			{
				index = 1;
			}
			for (int i = 0; i < SpriteSelectors.Length; i++)
			{
				SpriteSelectors[i].SelectSprite(index);
			}
			for (int i = 0; i < TintSelectors.Length; i++)
			{
				TintSelectors[i].SelectColor(index);
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RaceGameEvents.RaceFinishPopupClosed(raceResults.trackId));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
		}

		private IEnumerator loadRankEntryPrefabs()
		{
			AssetRequest<GameObject> rankEntryPrefabAsset = Content.LoadAsync(RankEntryPrefabKey);
			yield return rankEntryPrefabAsset;
			rankEntryPrefab = rankEntryPrefabAsset.Asset;
			AssetRequest<GameObject> rankEntrySelectedPrefabAsset = Content.LoadAsync(RankEntrySelectedPrefabKey);
			yield return rankEntrySelectedPrefabAsset;
			rankEntrySelectedPrefab = rankEntrySelectedPrefabAsset.Asset;
			loadRankEntries();
		}

		private void loadRankEntries()
		{
			long raceTime = raceResults.CompletionTime - raceResults.StartTime;
			addRankEntry(raceTime, RaceResults.RaceResultsCategory.Legendary, raceResults.raceResultsCategory == RaceResults.RaceResultsCategory.Legendary);
			addRankEntry(raceTime, RaceResults.RaceResultsCategory.Gold, raceResults.raceResultsCategory == RaceResults.RaceResultsCategory.Gold);
			addRankEntry(raceTime, RaceResults.RaceResultsCategory.Silver, raceResults.raceResultsCategory == RaceResults.RaceResultsCategory.Silver);
			addRankEntry(raceTime, RaceResults.RaceResultsCategory.Bronze, raceResults.raceResultsCategory == RaceResults.RaceResultsCategory.Bronze);
		}

		private void addRankEntry(long raceTime, RaceResults.RaceResultsCategory category, bool isSelected = false)
		{
			GameObject gameObject = (!isSelected) ? Object.Instantiate(rankEntryPrefab, RankEntryParent.transform, false) : Object.Instantiate(rankEntrySelectedPrefab, RankEntryParent.transform, false);
			gameObject.GetComponent<SledRacerFinishPopupRankEntry>().Initialize(raceTime, category, rankTimes, isSelected);
		}
	}
}
