using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class MascotDialogOverride : FsmStateAction
	{
		public string MascotName;

		public DialogList Dialog;

		private DialogList prevDialog;

		public override void OnEnter()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(MascotName);
			prevDialog = mascot.ActiveQuestDialog;
			mascot.ActiveQuestDialog = Dialog;
			Finish();
		}

		public override void OnExit()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(MascotName);
			mascot.ActiveQuestDialog = prevDialog;
		}
	}
}
