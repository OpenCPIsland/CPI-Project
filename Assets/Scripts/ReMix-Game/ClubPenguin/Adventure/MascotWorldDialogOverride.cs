using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class MascotWorldDialogOverride : FsmStateAction
	{
		public string MascotName;

		public DialogList Dialog;

		private DialogList prevDialog;

		public override void OnEnter()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(MascotName);
			mascot.RandomWorldDialogOverride = Dialog;
			Finish();
		}

		public override void OnExit()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(MascotName);
			mascot.RandomWorldDialogOverride = null;
		}
	}
}
