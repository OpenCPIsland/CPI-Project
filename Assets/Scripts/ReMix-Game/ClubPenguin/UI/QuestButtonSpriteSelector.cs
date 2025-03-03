using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(MainNavButton))]
	public class QuestButtonSpriteSelector : MonoBehaviour
	{
		private static SpriteContentKey starIconContentKey = new SpriteContentKey("Images/Quests_MainNavStar_*");

		public Image starIcon;

		private EventChannel eventChannel;

		public void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
		}

		public void Start()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				loadSprite(activeQuest);
			}
			eventChannel.AddListener<QuestEvents.StartQuest>(onQuestStarted);
			eventChannel.AddListener<QuestEvents.ReplayQuest>(onQuestReplayed);
			eventChannel.AddListener<QuestEvents.ResumeQuest>(onQuestResumed);
			eventChannel.AddListener<QuestEvents.SuspendQuest>(onQuestSuspended);
			eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		public void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void OnButtonEnabled()
		{
			if (Service.Get<QuestService>().ActiveQuest != null)
			{
				starIcon.gameObject.SetActive(true);
			}
		}

		public void OnButtonDisabled()
		{
			starIcon.gameObject.SetActive(false);
		}

		private bool onQuestStarted(QuestEvents.StartQuest evt)
		{
			loadSprite(evt.Quest);
			return false;
		}

		private bool onQuestReplayed(QuestEvents.ReplayQuest evt)
		{
			loadSprite(evt.Quest);
			return false;
		}

		private bool onQuestResumed(QuestEvents.ResumeQuest evt)
		{
			loadSprite(evt.Quest);
			return false;
		}

		private bool onQuestSuspended(QuestEvents.SuspendQuest evt)
		{
			starIcon.gameObject.SetActive(false);
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State == Quest.QuestState.Completed)
			{
				starIcon.gameObject.SetActive(false);
			}
			else if (evt.Quest.State == Quest.QuestState.Active)
			{
				loadSprite(evt.Quest);
			}
			return false;
		}

		private void loadSprite(Quest quest)
		{
			Content.LoadAsync(onSpriteLoaded, starIconContentKey, quest.Mascot.AbbreviatedName);
		}

		private void onSpriteLoaded(string path, Sprite sprite)
		{
			if (!(base.gameObject == null))
			{
				if (GetComponent<Button>().interactable)
				{
					starIcon.gameObject.SetActive(true);
				}
				starIcon.sprite = sprite;
			}
		}
	}
}
