using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowRemotePlayersAction : FsmStateAction
	{
		public override void OnEnter()
		{
			RemotePlayerVisibilityState.ShowRemotePlayers();
			Finish();
		}
	}
}
