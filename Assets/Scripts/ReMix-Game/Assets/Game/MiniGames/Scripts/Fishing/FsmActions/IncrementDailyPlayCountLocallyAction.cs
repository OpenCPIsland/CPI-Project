using HutongGames.PlayMaker;

namespace Assets.Game.MiniGames.Scripts.Fishing.FsmActions
{
	public class IncrementDailyPlayCountLocallyAction : FsmStateAction
	{
		public FsmString GameId = "fishing";

		public override void OnEnter()
		{
			Finish();
		}
	}
}
