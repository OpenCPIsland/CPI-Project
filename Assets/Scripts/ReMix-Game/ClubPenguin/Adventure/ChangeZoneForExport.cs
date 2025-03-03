using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	[Tooltip("Change the zone during the export process, so data can be loaded from the correct scene for other validation")]
	public class ChangeZoneForExport : FsmStateAction, ExportTransitionAction
	{
		[RequiredField]
		public ZoneDefinition Zone;

		public void CompleteTransitionAction()
		{
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
