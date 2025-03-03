using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class ActiveSelectionState : AbstractInputInteractionState
	{
		public GameObject CurrentlySelectedObject;

		public event Action<GameObject> TouchPhaseEnded;

		public event Action<GameObject, Vector2> TouchPhaseMoved;

		public ActiveSelectionState()
		{
			state = InteractionState.ActiveSelectedItem;
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
			if (!(CurrentlySelectedObject != null))
			{
				return;
			}
			GameObject gameObject = null;
			switch (touch.phase)
			{
			case TouchPhase.Began:
				break;
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Ended:
				gameObject = raycastScreenPointToObject(touch.position, TargetLayerMask);
				if (gameObject == null || gameObject == CurrentlySelectedObject)
				{
					gameObject = null;
				}
				if (this.TouchPhaseEnded != null)
				{
					this.TouchPhaseEnded(gameObject);
				}
				break;
			case TouchPhase.Moved:
				gameObject = raycastScreenPointToObject(touch.position, TargetLayerMask);
				if (gameObject == null || gameObject != CurrentlySelectedObject)
				{
					gameObject = null;
				}
				if (this.TouchPhaseMoved != null)
				{
					this.TouchPhaseMoved(gameObject, touch.position);
				}
				break;
			}
		}
	}
}
