using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Tutorial
{
	[ActionCategory("Tutorial")]
	public class EndTutorialAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<TutorialManager>().EndTutorial();
			Finish();
		}
	}
}
