using Disney.Kelowna.Common;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class DisabledInputState : AbstractInputInteractionState
	{
		public DisabledInputState()
		{
			state = InteractionState.DisabledInput;
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
		}
	}
}
