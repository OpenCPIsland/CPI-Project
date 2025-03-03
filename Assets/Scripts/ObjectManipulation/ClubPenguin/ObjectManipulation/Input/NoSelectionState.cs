using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class NoSelectionState : AbstractInputInteractionState
	{
		private float timeBeganTouch = 0f;

		public event Action<GameObject, Vector2> TouchPhaseEnded;

		public event Action<Vector2> TouchPhaseMoved;

		public NoSelectionState()
		{
			state = InteractionState.NoSelectedItem;
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
			GameObject gameObject = null;
			switch (touch.phase)
			{
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Canceled:
				break;
			case TouchPhase.Began:
				timeBeganTouch = Time.time;
				break;
			case TouchPhase.Ended:
				gameObject = raycastScreenPointToObject(touch.position, TargetLayerMask);
				if (gameObject != null && this.TouchPhaseEnded != null)
				{
					this.TouchPhaseEnded(gameObject, touch.position);
				}
				break;
			case TouchPhase.Moved:
			{
				float num = Time.time - timeBeganTouch;
				if (num >= MinTimeToMoveInput)
				{
					if (this.TouchPhaseMoved != null)
					{
						this.TouchPhaseMoved(touch.deltaPosition);
					}
				}
				else if (touch.deltaPosition.magnitude > 14f && this.TouchPhaseMoved != null)
				{
					this.TouchPhaseMoved(touch.deltaPosition);
				}
				break;
			}
			}
		}
	}
}
