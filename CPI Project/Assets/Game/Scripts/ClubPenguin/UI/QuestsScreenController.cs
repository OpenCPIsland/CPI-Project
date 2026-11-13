using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestsScreenController : MonoBehaviour
	{
		private StateMachineContext trayFSMContext;

		public string CurrentMascotID
		{
			get;
			set;
		}

		public MascotDefinition.QuestChapterData CurrentChapterData
		{
			get;
			set;
		}

		public int CurrentChapterNumber
		{
			get
			{
				return CurrentChapterData.Number;
			}
		}

		private void Start()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			trayFSMContext = gameObject.GetComponent<StateMachineContext>();
			if (Service.Get<MascotService>().ActiveMascot != null)
			{
				trayFSMContext.SendEvent(new ExternalEvent("ScreenQuests", "adventures"));
			}
			else if (Service.Get<QuestService>().ActiveQuest != null)
			{
				trayFSMContext.SendEvent(new ExternalEvent("ScreenQuests", "details"));
			}
			else
			{
				trayFSMContext.SendEvent(new ExternalEvent("ScreenQuests", "home"));
			}
			Service.Get<EventDispatcher>().AddListener<QuestScreenEvents.ShowQuestLogAdventures>(onShowQuestLogAdventures);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<QuestScreenEvents.ShowQuestLogAdventures>(onShowQuestLogAdventures);
		}

		private bool onShowQuestLogAdventures(QuestScreenEvents.ShowQuestLogAdventures evt)
		{
			CurrentMascotID = evt.MascotID;
			return false;
		}
	}
}
