using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class PlayMascotAnimationAction : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject Mascot;

		[RequiredField]
		public FsmString AnimationTrigger;

		public override void OnEnter()
		{
			MascotController component = Mascot.Value.GetComponent<MascotController>();
			if (component != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CinematicSpeechEvents.PlayMascotAnimation(component, AnimationTrigger.Value));
			}
			Finish();
		}
	}
}
