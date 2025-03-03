using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	[Tooltip("Use this as you would the \"Create Object\" action for any objects that the server needs to know about for exporting verified actions.  This will not create the object for you in normal game play.  This does not count as a server verifiable action by itself.")]
	public class CreateObjectForExport : CreateObject, ExportTransitionAction
	{
		public override void OnEnter()
		{
			Finish();
		}

		public void CompleteTransitionAction()
		{
			base.OnEnter();
		}
	}
}
