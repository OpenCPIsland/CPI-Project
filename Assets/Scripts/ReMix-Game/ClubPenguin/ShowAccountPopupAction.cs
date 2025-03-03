using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class ShowAccountPopupAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<SessionEvents.FTUENameObjectiveCompleteEvent>(onNameObjectiveComplete);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.FTUENameObjectiveCancelledEvent>(onNameObjectiveCancelled);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.FTUENameObjectiveAlreadyDoneEvent>(onNameObjectiveAlreadyDone);
			Service.Get<GameStateController>().ShowAccountSystemFromFTUE();
		}

		private bool onNameObjectiveComplete(SessionEvents.FTUENameObjectiveCompleteEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCompleteEvent>(onNameObjectiveComplete);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCancelledEvent>(onNameObjectiveCancelled);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveAlreadyDoneEvent>(onNameObjectiveAlreadyDone);
			Finish();
			return false;
		}

		private bool onNameObjectiveCancelled(SessionEvents.FTUENameObjectiveCancelledEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCompleteEvent>(onNameObjectiveComplete);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCancelledEvent>(onNameObjectiveCancelled);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveAlreadyDoneEvent>(onNameObjectiveAlreadyDone);
			base.Fsm.Event("Cancelled");
			Finish();
			return false;
		}

		private bool onNameObjectiveAlreadyDone(SessionEvents.FTUENameObjectiveAlreadyDoneEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCompleteEvent>(onNameObjectiveComplete);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveCancelledEvent>(onNameObjectiveCancelled);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.FTUENameObjectiveAlreadyDoneEvent>(onNameObjectiveAlreadyDone);
			base.Fsm.Event("AlreadyFinishedFTUE");
			Finish();
			return false;
		}
	}
}
