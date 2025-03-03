using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowQuestItemPopupAction : FsmStateAction
	{
		public string Message = "You got Stinky Cheese!";

		public string i18nMessage = "";

		public string PopupPrefabOverrideKey = "";

		public string NotificationMessage = "";

		public string i18nNotificationMessage = "";

		public bool WaitForPopupComplete = true;

		public float OpenDelay = 0f;

		private PrefabContentKey popupContentKey = new PrefabContentKey("Prefabs/Quest/Popups/FoundQuestItemPopup*");

		public QuestItemPopupImageInfo[] imageInfos = new QuestItemPopupImageInfo[0];

		private GameObject popup;

		public int numImages = 1;

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(PopupPrefabOverrideKey))
			{
				Content.LoadAsync(onPrefabLoaded, new PrefabContentKey(PopupPrefabOverrideKey));
				return;
			}
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				Content.LoadAsync(onPrefabLoaded, new PrefabContentKey(popupContentKey, activeQuest.Mascot.AbbreviatedName));
			}
			else
			{
				Finish();
			}
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			DQuestItemPopup dQuestItemPopup = new DQuestItemPopup();
			dQuestItemPopup.Message = Service.Get<Localizer>().GetTokenTranslation(i18nMessage);
			dQuestItemPopup.NotificationMessage = Service.Get<Localizer>().GetTokenTranslation(i18nNotificationMessage);
			dQuestItemPopup.ItemInfos = imageInfos;
			popup = Object.Instantiate(prefab);
			QuestItemPopup component = popup.GetComponent<QuestItemPopup>();
			if (component != null)
			{
				component.SetData(dQuestItemPopup);
				component.OpenDelay = OpenDelay;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup, false, true, "Accessibility.Popup.Title.QuestItem"));
			if (WaitForPopupComplete)
			{
				component.DoneClose += onPopupClosed;
			}
			else
			{
				Finish();
			}
		}

		private void onPopupClosed()
		{
			popup.GetComponent<QuestItemPopup>().DoneClose -= onPopupClosed;
			Finish();
		}
	}
}
