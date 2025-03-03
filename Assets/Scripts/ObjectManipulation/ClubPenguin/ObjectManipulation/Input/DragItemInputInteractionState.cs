using Disney.Kelowna.Common;
using System;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class DragItemInputInteractionState : AbstractInputInteractionState
	{
		public Vector2 TouchOffset = Vector2.zero;

		public GameObject CurrentlySelectedObject;

		public RaycastHit[] RaycastHits;

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseEnded;

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseMoved;

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseStationary;

		public DragItemInputInteractionState()
		{
			state = InteractionState.DragItem;
		}

		public override void EnterState(LayerMask targetLayerMask, float minTimeToMoveInput)
		{
			base.EnterState(targetLayerMask, minTimeToMoveInput);
			if (Update() == 0)
			{
				this.TouchPhaseEnded.InvokeSafe(default(RaycastHit), default(TouchEquivalent));
			}
		}

		public override void ExitState()
		{
			base.ExitState();
			TouchOffset = Vector2.zero;
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
			if (!(CurrentlySelectedObject != null))
			{
				return;
			}
			switch (touch.phase)
			{
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (this.TouchPhaseEnded != null)
				{
					RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.position, TargetLayerMask);
					this.TouchPhaseEnded(arg, touch);
				}
				break;
			case TouchPhase.Stationary:
				StationaryRaycast(touch);
				break;
			case TouchPhase.Moved:
			{
				RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.position, TargetLayerMask);
				if (!arg.Equals(default(RaycastHit)))
				{
					this.TouchPhaseMoved.InvokeSafe(arg, touch);
				}
				break;
			}
			}
		}

		private void StationaryRaycast(TouchEquivalent touch)
		{
			RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.position, TargetLayerMask);
			if (!arg.Equals(default(RaycastHit)))
			{
				this.TouchPhaseStationary.InvokeSafe(arg, touch);
			}
		}

		private RaycastHit raycastTopmostItemFromScreenPoint(Vector2 screenPosition, LayerMask layerMask)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenPosition + TouchOffset);
			RaycastHit result = default(RaycastHit);
			RaycastHits = (from hit in Physics.RaycastAll(ray, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore)
				orderby hit.distance
				select hit).ToArray();
			for (int i = 0; i < RaycastHits.Length; i++)
			{
				GameObject gameObject = RaycastHits[i].collider.gameObject;
				if (gameObject != CurrentlySelectedObject && !gameObject.transform.IsChildOf(CurrentlySelectedObject.transform))
				{
					result = RaycastHits[i];
					break;
				}
			}
			if (result.transform != null)
			{
			}
			return result;
		}
	}
}
