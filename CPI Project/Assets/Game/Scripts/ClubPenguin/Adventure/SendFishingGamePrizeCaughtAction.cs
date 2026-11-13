using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Deprecated")]
	public class SendFishingGamePrizeCaughtAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Finish();
		}
	}
}
