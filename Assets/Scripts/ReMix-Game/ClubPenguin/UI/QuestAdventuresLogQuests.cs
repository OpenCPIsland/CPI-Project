using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestAdventuresLogQuests : MonoBehaviour
	{
		public ScrollRect ScrollRect;

		public Transform ScrollContent;

		public Text NoQuestsText;

		public GameObject LogPanel;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private static PrefabContentKey questItemContentKey = new PrefabContentKey("ScreenQuestsAdventuresPrefabs/QuestLogItem_*");

		private static PrefabContentKey comingSoonContentKey = new PrefabContentKey("ScreenQuestsAdventuresPrefabs/QuestLogItem_*_ComingSoon");

		private int totalAdventuresItems = 0;

		private int adventureItemsLoaded = 0;

		private float scrollPosition = 0f;

		private Mascot currentMascot;

		private MascotDefinition.QuestChapterData currentChapter;

		private QuestsScreenController questsScreenController;

		private void Start()
		{
			questsScreenController = GetComponentInParent<QuestsScreenController>();
			questsScreenController.GetComponent<MainNavBarBackButtonEventToFSMEvent>().enabled = false;
			currentMascot = Service.Get<MascotService>().ActiveMascot;
			if (currentMascot == null)
			{
				string currentMascotID = questsScreenController.CurrentMascotID;
				if (!string.IsNullOrEmpty(currentMascotID))
				{
					currentMascot = Service.Get<MascotService>().GetMascot(currentMascotID);
				}
			}
			currentChapter = questsScreenController.CurrentChapterData;
			if (currentMascot != null)
			{
				if (currentChapter.Number > 0)
				{
					LoadQuestsForMascot(currentMascot.Name, currentChapter.Number);
					return;
				}
				QuestDefinition nextAvailableQuest = currentMascot.GetNextAvailableQuest();
				int chapterNumber = (nextAvailableQuest != null) ? (nextAvailableQuest.ChapterNumber - 1) : currentMascot.Definition.ChapterData[currentMascot.Definition.ChapterData.Length - 1].Number;
				LoadQuestsForMascot(currentMascot.Name, chapterNumber);
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < currentMascot.KnownQuests.Length; i++)
			{
				Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(BreadcrumbType, currentMascot.KnownQuests[i].name);
			}
			Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(string.Format("{0}Quest", currentMascot.AbbreviatedName));
			questsScreenController.GetComponent<MainNavBarBackButtonEventToFSMEvent>().enabled = true;
		}

		public void LoadQuestsForMascot(string mascotID, int chapterNumber)
		{
			clearCurrentLog();
			Mascot mascot = Service.Get<MascotService>().GetMascot(mascotID);
			LogPanel.SetActive(true);
			QuestDefinition nextAvailableQuest = mascot.GetNextAvailableQuest(chapterNumber);
			int num = (nextAvailableQuest != null) ? (nextAvailableQuest.QuestNumber - 1) : (-1);
			List<QuestDefinition> validQuests = mascot.GetValidQuests(chapterNumber);
			totalAdventuresItems = validQuests.Count;
			adventureItemsLoaded = 0;
			if (num != -1)
			{
				scrollPosition = (float)num / (float)(totalAdventuresItems - 1);
			}
			else
			{
				scrollPosition = 0f;
			}
			for (int i = 0; i < validQuests.Count; i++)
			{
				CoroutineRunner.Start(loadQuestLogAdventureItem(validQuests[i]), this, "loadQuestLogAdventureItem");
			}
		}

		private IEnumerator loadQuestLogAdventureItem(QuestDefinition questDefinition)
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(questDefinition.Mascot.name);
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(questItemContentKey, mascot.AbbreviatedName);
			yield return assetRequest;
			GameObject questLogItemGameObject = Object.Instantiate(assetRequest.Asset);
			QuestLogAdventuresItem questLogItem = questLogItemGameObject.GetComponent<QuestLogAdventuresItem>();
			questLogItem.LoadQuestData(questDefinition);
			questLogItemGameObject.transform.SetParent(ScrollContent, false);
			adventureItemsLoaded++;
			if (adventureItemsLoaded == totalAdventuresItems)
			{
				CoroutineRunner.Start(scrollToAvailableQuest(), this, "");
				if (mascot.Definition.ShowComingSoonInLog)
				{
					AssetRequest<GameObject> assetRequest2 = Content.LoadAsync(comingSoonContentKey, mascot.AbbreviatedName);
					GameObject gameObject = Object.Instantiate(assetRequest2.Asset);
					gameObject.transform.SetParent(ScrollContent, false);
				}
			}
		}

		private IEnumerator scrollToAvailableQuest()
		{
			yield return new WaitForEndOfFrame();
			ScrollRect.horizontalNormalizedPosition = scrollPosition;
		}

		private void clearCurrentLog()
		{
			foreach (Transform item in ScrollContent)
			{
				Object.Destroy(item.gameObject);
			}
		}
	}
}
