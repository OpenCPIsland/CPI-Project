using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class SetFtueCompleteAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<GameStateController>().SetFTUEComplete();
			Finish();
		}
	}
}
