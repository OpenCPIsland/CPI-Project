using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SetMascotIdleDialogOverrideAction : FsmStateAction
	{
		[RequiredField]
		public MascotDefinition Mascot;

		public string AnimationTrigger;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new CinematicSpeechEvents.SetMascotIdleDialogOverride(Mascot.name, AnimationTrigger));
			Finish();
		}
	}
}
