using ClubPenguin.SledRacer;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForTubeRaceFinishPopupCloseAction : FsmStateAction
	{
		public string RaceTrackId;

		public FsmEvent ClosedEvent;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<RaceGameEvents.RaceFinishPopupClosed>(onRaceFinishPopupClosed);
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<RaceGameEvents.RaceFinishPopupClosed>(onRaceFinishPopupClosed);
		}

		private bool onRaceFinishPopupClosed(RaceGameEvents.RaceFinishPopupClosed evt)
		{
			if (string.IsNullOrEmpty(RaceTrackId) || evt.TrackId == RaceTrackId)
			{
				base.Fsm.Event(ClosedEvent);
			}
			return false;
		}
	}
}
