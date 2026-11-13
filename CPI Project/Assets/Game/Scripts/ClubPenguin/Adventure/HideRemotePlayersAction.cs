using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class HideRemotePlayersAction : FsmStateAction
	{
		public override void OnEnter()
		{
			RemotePlayerVisibilityState.HideRemotePlayers();
			Finish();
		}
	}
}
