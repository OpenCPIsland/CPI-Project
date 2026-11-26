#if UNITY_ANDROID || UNITY_IOS
using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation.Input
{
	public class DragItemInputInteractionState : AbstractInputInteractionState
	{
		public Vector2 TouchOffset = Vector2.zero;

		public GameObject CurrentlySelectedObject;

		public RaycastHit[] RaycastHits = new RaycastHit[0];

		// True when the last raycast was skipped because the ray (finger and camera) did not
		// move: the item is already where that hit puts it.
		public bool HitUnchanged;

		private const int MaxRaycastHits = 64;

		private readonly RaycastHit[] raycastBuffer = new RaycastHit[MaxRaycastHits];

		private static readonly IComparer<RaycastHit> byDistance = new RaycastHitDistanceComparer();

		private bool hasLastRay;

		private Ray lastRay;

		private RaycastHit lastResult;

		private class RaycastHitDistanceComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit a, RaycastHit b)
			{
				return a.distance.CompareTo(b.distance);
			}
		}

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseEnded;

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseMoved;

		public event Action<RaycastHit, TouchEquivalent> TouchPhaseStationary;

		public DragItemInputInteractionState()
		{
			state = InteractionState.DragItem;
		}

		// An item dragged out of the inventory panel starts its touch on the UI.
		protected override bool IgnoresTouchesBeganOverUI
		{
			get
			{
				return false;
			}
		}

		public override void EnterState(LayerMask targetLayerMask, float minTimeToMoveInput)
		{
			base.EnterState(targetLayerMask, minTimeToMoveInput);
			hasLastRay = false;
			HitUnchanged = false;
			if (Update() == 0)
			{
				this.TouchPhaseEnded.InvokeSafe(default(RaycastHit), default(TouchEquivalent));
			}
		}

		public override void ExitState()
		{
			base.ExitState();
			TouchOffset = Vector2.zero;
			hasLastRay = false;
			HitUnchanged = false;
		}

		protected override void processOneTouch(TouchEquivalent touch)
		{
			if (!(CurrentlySelectedObject != null))
			{
				return;
			}
			switch (touch.Phase)
			{
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (this.TouchPhaseEnded != null)
				{
					RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.Position, TargetLayerMask);
					this.TouchPhaseEnded(arg, touch);
				}
				break;
			case TouchPhase.Stationary:
				StationaryRaycast(touch);
				break;
			case TouchPhase.Moved:
			{
				RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.Position, TargetLayerMask);
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
			RaycastHit arg = raycastTopmostItemFromScreenPoint(touch.Position, TargetLayerMask);
			if (!arg.Equals(default(RaycastHit)))
			{
				this.TouchPhaseStationary.InvokeSafe(arg, touch);
			}
		}

		private RaycastHit raycastTopmostItemFromScreenPoint(Vector2 screenPosition, LayerMask layerMask)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenPosition + TouchOffset);
			// A still finger under a still camera hits the same thing: skip the physics query.
			if (hasLastRay && lastResult.transform != null && ray.origin == lastRay.origin && ray.direction == lastRay.direction)
			{
				HitUnchanged = true;
				return lastResult;
			}
			HitUnchanged = false;
			RaycastHit result = default(RaycastHit);
			// No per-frame allocation: a reused buffer sorted in place, copied out only when
			// the number of hits changes size.
			int count = Physics.RaycastNonAlloc(ray, raycastBuffer, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Ignore);
			Array.Sort(raycastBuffer, 0, count, byDistance);
			if (RaycastHits.Length != count)
			{
				RaycastHits = new RaycastHit[count];
			}
			Array.Copy(raycastBuffer, RaycastHits, count);
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = RaycastHits[i].collider.gameObject;
				if (gameObject != CurrentlySelectedObject && !gameObject.transform.IsChildOf(CurrentlySelectedObject.transform))
				{
					result = RaycastHits[i];
					break;
				}
			}
			lastRay = ray;
			lastResult = result;
			hasLastRay = true;
			return result;
		}
	}
}
#else
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
#endif
