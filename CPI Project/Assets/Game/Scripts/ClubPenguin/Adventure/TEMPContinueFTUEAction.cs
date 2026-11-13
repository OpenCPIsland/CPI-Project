using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class TEMPContinueFTUEAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<GameStateController>().ContinueFTUE();
			Finish();
		}
	}
}
