using ClubPenguin.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckMemberStatusAction : FsmStateAction
	{
		public FsmEvent IsMemberEvent;

		public FsmEvent NotMemberEvent;

		public override void OnEnter()
		{
			if (Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				base.Fsm.Event(IsMemberEvent);
			}
			else
			{
				base.Fsm.Event(NotMemberEvent);
			}
		}
	}
}
