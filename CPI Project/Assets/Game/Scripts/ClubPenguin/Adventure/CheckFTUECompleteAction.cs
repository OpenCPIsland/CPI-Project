using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckFTUECompleteAction : FsmStateAction
	{
		public FsmEvent CompleteEvent;

		public FsmEvent NotCompleteEvent;

		public override void OnEnter()
		{
			if (Service.Get<GameStateController>().IsFTUEComplete)
			{
				base.Fsm.Event(CompleteEvent);
			}
			else
			{
				base.Fsm.Event(NotCompleteEvent);
			}
			Finish();
		}
	}
}
