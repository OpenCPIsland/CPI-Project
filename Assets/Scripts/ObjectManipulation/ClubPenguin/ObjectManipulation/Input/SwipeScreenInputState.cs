using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class SwipeScreenInputState : AbstractInputInteractionState
	{
		public event Action TouchPhaseEnded;

		public event Action<Vector2> TouchPhaseMoved;

		public SwipeScreenInputState()
		{
			state = InteractionState.SwipeScreen;
		}

		public void ProcessMove(Vector2 deltaTouchPosition)
		{
			if (this.TouchPhaseMoved != null)
			{
				this.TouchPhaseMoved(deltaTouchPosition);
			}
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (this.TouchPhaseEnded != null)
				{
					this.TouchPhaseEnded();
				}
				break;
			case TouchPhase.Moved:
				if (this.TouchPhaseMoved != null)
				{
					this.TouchPhaseMoved(touch.deltaPosition);
				}
				break;
			}
		}
	}
}
