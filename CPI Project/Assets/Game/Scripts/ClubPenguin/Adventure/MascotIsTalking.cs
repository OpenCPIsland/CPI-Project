using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class MascotIsTalking : FsmStateAction
	{
		public string MascotName;

		public override void OnEnter()
		{
			Service.Get<MascotService>().GetMascot(MascotName).IsTalking = true;
			Finish();
		}

		public override void OnExit()
		{
			Service.Get<MascotService>().GetMascot(MascotName).IsTalking = false;
		}
	}
}
