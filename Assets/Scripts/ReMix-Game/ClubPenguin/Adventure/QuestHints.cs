using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestHints : MonoBehaviour
	{
		public const float QUEST_HINT_DISMISS_TIME = 4f;

		public const float QUEST_HINT_WAIT_TIME_SHORT = 30f;

		public const float QUEST_HINT_WAIT_TIME_LONG = 60f;

		public const float QUEST_HINT_DELAYED_WAIT_TIME = 2.5f;

		private const string QUEST_HINT_TIME_PREFS_KEY = "QUEST_HINT_TIME";

		private const string TRAY_CLOSE_STATE = "MinNPC";

		private const string QUEST_EVENT_NAME = "QuestHintShown";

		private QuestHint currentHint;

		private EventDispatcher dispatcher;

		private float hintTimer = 0f;

		private bool loadedHintTime = false;

		private QuestHintState hintState;

		private EventChannel eventChannel;

		private StateMachine trayFSM;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<QuestEvents.SetQuestHint>(setHint);
			eventChannel.AddListener<QuestEvents.CancelQuestHint>(cancelHint);
			if (PlayerPrefs.HasKey("QUEST_HINT_TIME"))
			{
				float @float = PlayerPrefs.GetFloat("QUEST_HINT_TIME");
				if (@float != 0f)
				{
					hintTimer = Mathf.Max(@float, 2.5f);
					hintState = QuestHintState.Waiting;
					loadedHintTime = true;
				}
				PlayerPrefs.DeleteKey("QUEST_HINT_TIME");
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (hintState == QuestHintState.Waiting)
			{
				PlayerPrefs.SetFloat("QUEST_HINT_TIME", hintTimer);
			}
		}

		private void Update()
		{
			if (hintState == QuestHintState.Waiting)
			{
				hintTimer -= Time.deltaTime;
				if (hintTimer <= 0f)
				{
					tryShowHint();
				}
			}
			else if (hintState == QuestHintState.Delayed && !checkTrayIsClosed())
			{
				hintState = QuestHintState.Waiting;
				hintTimer = 2.5f;
			}
		}

		private bool setHint(QuestEvents.SetQuestHint evt)
		{
			if (hintState == QuestHintState.Idle || loadedHintTime)
			{
				currentHint = evt.HintData;
				if (!loadedHintTime)
				{
					startHintTimer(currentHint);
				}
				loadedHintTime = false;
			}
			return false;
		}

		private bool cancelHint(QuestEvents.CancelQuestHint evt)
		{
			hintState = QuestHintState.Idle;
			return false;
		}

		private void tryShowHint()
		{
			if (!checkTrayIsClosed())
			{
				DQuestMessage dQuestMessage = new DQuestMessage();
				dQuestMessage.MascotName = currentHint.MascotName;
				dQuestMessage.DismissTime = 4f;
				dQuestMessage.Text = currentHint.HintText;
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.ShowQuestMessage(dQuestMessage));
				Service.Get<QuestService>().SendEvent("QuestHintShown");
				if (currentHint.Repeat)
				{
					startHintTimer(currentHint);
				}
				else
				{
					hintTimer = 0f;
					hintState = QuestHintState.Idle;
				}
			}
			else
			{
				hintState = QuestHintState.Delayed;
				hintTimer = 2.5f;
			}
			loadedHintTime = false;
		}

		private void startHintTimer(QuestHint hint)
		{
			float num = 0f;
			switch (hint.WaitType)
			{
			case QuestHintWaitType.HintTimeShort:
				num = 30f;
				break;
			case QuestHintWaitType.HintTimeLong:
				num = 60f;
				break;
			case QuestHintWaitType.HintTimeCustom:
				num = hint.WaitTime;
				break;
			}
			if (num != 0f)
			{
				hintTimer = num;
				hintState = QuestHintState.Waiting;
			}
		}

		private bool checkTrayIsClosed()
		{
			if (trayFSM == null)
			{
				trayFSM = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponent<StateMachine>();
			}
			return trayFSM.CurrentState.Name == "MinNPC";
		}
	}
}
