using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public abstract class InteractionZoneObserver : MonoBehaviour
	{
		protected EventDispatcher dispatcher;

		public void Start()
		{
			dispatcher = GetComponentInParent<InteractiveZoneController>().Dispatcher;
			if (dispatcher == null)
			{
				throw new InvalidOperationException("This class requires an EventDispatcher in the parent game object");
			}
			dispatcher.AddListener<InteractionZoneEvents.InteractionZoneEvent>(OnPlayerTriggerInteractionZone);
		}

		protected abstract bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt);

		public virtual void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<InteractionZoneEvents.InteractionZoneEvent>(OnPlayerTriggerInteractionZone);
			}
		}
	}
}
