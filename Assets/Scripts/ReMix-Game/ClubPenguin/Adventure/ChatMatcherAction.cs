using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ChatMatcherAction : FsmStateAction
	{
		public string Expression;

		public FsmEvent MatchEvent;

		public FsmEvent FailedMatchEvent;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<ChatMessageSender.SendChatMessage>(OnChatMessage);
		}

		private bool OnChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			if (evt.Message.Trim() == Expression.Trim())
			{
				evt.Quest.QuestId = Service.Get<QuestService>().ActiveQuest.Id;
				evt.Quest.Objective = base.State.Name;
				base.Fsm.Event(MatchEvent);
			}
			else
			{
				base.Fsm.Event(FailedMatchEvent);
			}
			return false;
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<ChatMessageSender.SendChatMessage>(OnChatMessage);
		}
	}
}
