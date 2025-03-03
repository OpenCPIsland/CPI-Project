using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class SetInteractionButtonCommand : FsmStateAction
	{
		public bool ButtonOn;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PenguinInteraction.InteractChangeEvent(ButtonOn));
			Finish();
		}
	}
}
