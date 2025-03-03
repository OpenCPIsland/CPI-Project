using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestInventoryScreen : MonoBehaviour
	{
		public Text AdventureNameText;

		public Transform MascotScreenParent;

		private QuestDetailsMascotScreen mascotScreen;

		private Quest questData;

		private CoroutineGroup loadingCoroutines = new CoroutineGroup();

		private bool hasOpened = false;

		private static readonly PrefabContentKey defaultInventoryItemContentKey = new PrefabContentKey("Prefabs/Quest/QuestInventoryItems/QuestInventoryItem");

		private readonly PrefabContentKey inventoryScreenKey = new PrefabContentKey("ScreenQuestsDetailsPrefabs/AdventureInventoryScreen*");

		public void Start()
		{
			Service.Get<EventDispatcher>().AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			LoadQuestData(activeQuest);
			Service.Get<BackButtonController>().Add(onBackButtonClicked);
			MarketplaceScreenIntroComplete();
			Service.Get<ICPSwrveService>().Action("view.adventure_log");
			if (!activeQuest.Definition.IsPausable)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("QuestPauseButton"));
			}
		}

		public void OnDestroy()
		{
			Service.Get<BackButtonController>().Remove(onBackButtonClicked);
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			loadingCoroutines.StopAll();
		}

		public void LoadQuestData(Quest questData)
		{
			this.questData = questData;
			loadQuestMascotScreen(this.questData);
			AdventureNameText.text = Service.Get<Localizer>().GetTokenTranslation(questData.Definition.Title);
		}

		private void loadQuestMascotScreen(Quest questData)
		{
			Content.LoadAsync(onMascotScreenLoaded, inventoryScreenKey, questData.Mascot.AbbreviatedName);
		}

		private void onMascotScreenLoaded(string path, GameObject mascotScreenPrefab)
		{
			GameObject gameObject = Object.Instantiate(mascotScreenPrefab);
			gameObject.transform.SetParent(MascotScreenParent, false);
			mascotScreen = gameObject.GetComponent<QuestDetailsMascotScreen>();
			loadQuestInventory(questData);
		}

		private void loadQuestInventory(Quest questData)
		{
			clearCurrentInventory();
			loadingCoroutines.StopAll();
			bool flag = false;
			foreach (KeyValuePair<string, QuestItem> questItem in questData.QuestItems)
			{
				if (questItem.Value.State == QuestItem.QuestItemState.Collected || (questItem.Value.State == QuestItem.QuestItemState.NotCollected && !questItem.Value.DataModel.IsHiddenWhenNotCollected) || questItem.Value.State == QuestItem.QuestItemState.Interactive)
				{
					loadingCoroutines.StartAndAdd(loadQuestInventoryItem(questItem.Value), this, "loadQuestInventoryItem");
					flag = true;
				}
			}
			mascotScreen.NoItemsGraphic.SetActive(!flag);
		}

		private IEnumerator loadQuestInventoryItem(QuestItem questItem)
		{
			PrefabContentKey inventoryItemContentKey = questItem.DataModel.ItemUIContentKey;
			if (inventoryItemContentKey == null || string.IsNullOrEmpty(inventoryItemContentKey.Key))
			{
				inventoryItemContentKey = defaultInventoryItemContentKey;
			}
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(inventoryItemContentKey);
			yield return assetRequest;
			GameObject questInventoryGameObject = Object.Instantiate(assetRequest.Asset);
			QuestInventoryItem questInventoryItem = questInventoryGameObject.GetComponent<QuestInventoryItem>();
			questInventoryItem.LoadQuestItem(questItem);
			questInventoryGameObject.transform.SetParent(mascotScreen.QuestInventoryContentTransform, false);
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.Definition.Title == questData.Definition.Title)
			{
				questData = evt.Quest;
				loadQuestInventory(questData);
			}
			return false;
		}

		private void clearCurrentInventory()
		{
			foreach (Transform item in mascotScreen.QuestInventoryContentTransform)
			{
				Object.Destroy(item.gameObject);
			}
		}

		private void onBackButtonClicked()
		{
			CloseButtonPressed();
		}

		public void PauseButtonPressed()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SuspendQuest(Service.Get<QuestService>().ActiveQuest));
			Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
			Object.Destroy(base.gameObject);
		}

		public void CloseButtonPressed()
		{
			MarketplaceScreenOutroComplete();
		}

		public void MarketplaceScreenIntroComplete()
		{
			if (!hasOpened)
			{
				hasOpened = true;
			}
		}

		public void MarketplaceScreenOutroComplete()
		{
			if (hasOpened)
			{
				Object.Destroy(base.gameObject);
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
				component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
			}
		}
	}
}
