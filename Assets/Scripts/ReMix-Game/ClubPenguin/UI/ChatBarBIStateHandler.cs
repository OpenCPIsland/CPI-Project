using ClubPenguin.Analytics;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	public class ChatBarBIStateHandler : PassiveStateHandler
	{
		private const string KEYBOARD = "Default";

		private const string INSTANT = "Instant";

		private const string EMOTE_INPUT = "EmoteInput";

		private const string EMOTE_INSTANT = "EmoteInstant";

		private static string ANALYTIC_TIMER_QUICK_CHAT = "quick_chat";

		private static string SWRVE_ACTION_CHAT_UI = "game.chat_UI";

		private string currentState;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Action("game.chat_start");
			Service.Get<ICPSwrveService>().StartTimer("chat", "chat", SceneManager.GetActiveScene().name);
			Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayOpened));
		}

		public override void HandleStateChange(string newState)
		{
			bool previousStateWasInstant = currentState == "Instant" || currentState == "EmoteInstant";
			currentState = newState;
			switch (newState)
			{
			case "Instant":
			case "EmoteInstant":
				handleInstantState(previousStateWasInstant);
				break;
			case "EmoteInput":
			case "Default":
				handleInputState();
				break;
			}
		}

		private void handleInstantState(bool previousStateWasInstant)
		{
			if (!previousStateWasInstant)
			{
				Service.Get<ICPSwrveService>().StartTimer(ANALYTIC_TIMER_QUICK_CHAT, ANALYTIC_TIMER_QUICK_CHAT, SceneManager.GetActiveScene().name);
			}
			string tier = (currentState == "EmoteInstant") ? "quick_emoji" : "quick_chat";
			Service.Get<ICPSwrveService>().Action(SWRVE_ACTION_CHAT_UI, tier);
		}

		private void handleInputState()
		{
			Service.Get<ICPSwrveService>().EndTimer(ANALYTIC_TIMER_QUICK_CHAT);
			string tier = (currentState == "EmoteInput") ? "emoji" : "keyboard";
			Service.Get<ICPSwrveService>().Action(SWRVE_ACTION_CHAT_UI, tier);
		}

		private void OnDestroy()
		{
			Service.Get<ICPSwrveService>().EndTimer(ANALYTIC_TIMER_QUICK_CHAT);
			Service.Get<ICPSwrveService>().EndTimer("chat");
			Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayClosed));
		}
	}
}
